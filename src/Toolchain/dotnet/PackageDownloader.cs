using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Zgo.Core;
using System.IO.Compression;
internal class PackageDownloader
{


    public static async Task DownloadAndExtractPackageAsync(string packageId, string version, string targetDirectory)
    {
        // 1. 创建 NuGet 资源库（以官方源为例）
        ILogger logger = NullLogger.Instance;
        var cache = new SourceCacheContext();
        var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

        // 2. 获取资源
        var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

        // 3. 检查目标目录是否存在，若不存在则创建
        if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

        // 4. 获取所有可用版本并找到指定版本
        var allVersions = await resource.GetAllVersionsAsync(packageId, cache, logger, CancellationToken.None);
        var packageVersion = allVersions.FirstOrDefault(v => v.ToNormalizedString() == version);
        if (packageVersion == null)
        {
            throw new ArgumentException($"Version {version} of package {packageId} not found.");
        }

        // 5. 将包内容复制到目标目录（这实际上会下载并解压）
        using (var packageStream = new MemoryStream())
        {
            // 将包下载到内存流
            await resource.CopyNupkgToStreamAsync(packageId, packageVersion, packageStream, cache, logger, CancellationToken.None);
            packageStream.Seek(0, SeekOrigin.Begin);

            // 将内存流中的内容（.nupkg 压缩包）解压到目标目录
            using (var archive = new System.IO.Compression.ZipArchive(packageStream))
            {
                foreach (var entry in archive.Entries)
                {
                    var entryTargetPath = Path.Combine(targetDirectory, entry.FullName);
                    var entryTargetDir = Path.GetDirectoryName(entryTargetPath);
                    if(!Directory.Exists(entryTargetDir))
                        Directory.CreateDirectory(entryTargetDir);
                    if (!String.IsNullOrEmpty(entry.Name))
                    {
                        entry.ExtractToFile(entryTargetPath, overwrite: true);
                    }
                }
            }
        }
        Console.WriteLine($"Package {packageId}.{version} has been downloaded and extracted to {targetDirectory}");
    }
}

internal class PackageDownloadCommand : ZgoCommand
{
    [Option("--id", Description = "package id", Required = true)]
    private string _id = string.Empty;
    [Option("--version", Description = "package version", Required = true)]
    private string _version;
    [Option("--output", Description = "package save path", Required = true)]
    private string _outputPath;
    public PackageDownloadCommand() : base("nuget")
    { }

    protected override async Task OnExecuteAsync()
    {
        await PackageDownloader.DownloadAndExtractPackageAsync(_id, _version, _outputPath);
    }
}