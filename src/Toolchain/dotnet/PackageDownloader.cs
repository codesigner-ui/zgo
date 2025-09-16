using Zgo.Core;
internal class PackageDownloader
{

}

internal class PackageDownloadCommand : ZgoCommand
{
    [Option("--id", Description = "package id")]
    private string _id = "test";
    [Option("--package-version", Description = "package version")]
    private string _version;
    private bool _outputPath;
    private bool _bUnzip;
    public PackageDownloadCommand() : base("package")
    { }

    protected override void OnExecute()
    {
        ILogger logger = NullLogger.Instance;
        CancellationToken cancellationToken = CancellationToken.None;

        SourceCacheContext cache = new SourceCacheContext();
        SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

        string packageId = "Newtonsoft.Json";
        NuGetVersion packageVersion = new NuGetVersion("12.0.1");
        using MemoryStream packageStream = new MemoryStream();

        await resource.CopyNupkgToStreamAsync(
            packageId,
            packageVersion,
            packageStream,
            cache,
            logger,
            cancellationToken);

        Console.WriteLine($"Downloaded package {packageId} {packageVersion}");

        using PackageArchiveReader packageReader = new PackageArchiveReader(packageStream);
        NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

        Console.WriteLine($"Tags: {nuspecReader.GetTags()}");
        Console.WriteLine($"Description: {nuspecReader.GetDescription()}");
    }
}