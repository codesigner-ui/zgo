using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Zgo.Core;
using Zgo.Toolchain;

using System;
using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
namespace Zgo.Toolchain;

internal class DotnetInstallCommand : ZgoCommand
{
    [Option("--output", Description = "install directory")]
    private DirectoryInfo _outputDir;

    [Option("--sdk-version", Description = "sdk version")]
    private string _sdkVersion;

    [Option("--rid-version", Description = "app runtime rid.version, include rid, like: win-x64.9.0.9")]
    private string _ridVersion;


    [Option("--install-workload", Description = "install target workload for runtime rid build")]
    private bool _installWorkload;
    public DotnetInstallCommand() : base("install", "install sdk, runtime and workload")
    {
    }

    protected override async Task OnExecuteAsync()
    {
        if (!string.IsNullOrEmpty(_sdkVersion))
        {
            string sdkDir = Path.Combine(_outputDir.FullName, "sdk");
            await DotnetInstaller.InstallSdkAsync(_sdkVersion, sdkDir);
        }
        if (!string.IsNullOrEmpty(_ridVersion))
        {
            string runtimeDir = Path.Combine(_outputDir.FullName, "runtime");
            await DotnetInstaller.InstallRuntimeAsync(_ridVersion, runtimeDir);
        }
    }
}

internal class DotnetInstaller
{
    public static string GetHostSdkRid()
    {
        Architecture arch = RuntimeInformation.OSArchitecture;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return $"win-{arch.ToString().ToLower()}";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return $"linux-{arch.ToString().ToLower()}";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return $"osx-{arch.ToString().ToLower()}";
        }
        else
        {
            Console.WriteLine("当前操作系统是：其他未知平台");
        }
        return string.Empty;
    }

    public static async Task DownloadSdkAsync(string version, string rid, string outputDir)
    {
        string packageSuffix = ".zip";
        bool isZip = true;
        if (!rid.StartsWith("win"))
        {
            packageSuffix = ".tar.gz";
            isZip = false;
        }
        string downloadUrl = $"https://builds.dotnet.microsoft.com/dotnet/Sdk/{version}/dotnet-sdk-{version}-{rid}{packageSuffix}";
        string localFilePath = Path.Combine(outputDir, $"dotnet-sdk-{version}-{rid}{packageSuffix}");
        await FileDownloader.DownloadFileAsync(downloadUrl, localFilePath);
        if (isZip)
        {
            FileCompression.ExtractZip(localFilePath, outputDir);
        }
        else
        {
            FileCompression.ExtractTarGz(localFilePath, outputDir);
        }
        File.Delete(localFilePath);
    }

    public static async Task InstallSdkAsync(string version, string outputDir)
    {
        string hostRid = GetHostSdkRid();
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
        await DownloadSdkAsync(version, hostRid, outputDir);
    }

    public static async Task InstallRuntimeAsync(string version, string outputDir)
    {
        int index = version.IndexOf('.');
        string runtimeRid = version.Substring(0, index);
        string runtimeVersion = version.Substring(index + 1);
        string appRuntime = $"Microsoft.NETCore.App.Runtime.{runtimeRid}";
        outputDir = Path.Combine(outputDir, version);
        await PackageDownloader.DownloadAndExtractPackageAsync(appRuntime, runtimeVersion, outputDir);
    }

    public static void InstallWorkload(string version, string sdkDir)
    {
        
    }
}