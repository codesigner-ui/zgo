using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Zgo.Core;
using Zgo.Toolchain;

internal class DotnetInstallCommand : ZgoCommand
{
    [Option("--output", Description = "install directory")]
    private DirectoryInfo _outputDir;

    [Option("--sdk-version", Description = "sdk version")]
    private string _sdkVersion;

    [Option("--runtime-version", Description = "runtime version, include rid, like: win-x64.9.0.9")]
    private List<string> _runtimeVersions;


    [Option("--install-workload", Description = "install target workload for runtime rid build")]
    private bool _installWorkload;
    public DotnetInstallCommand() : base("install", "install sdk, runtime and workload")
    {
    }

    protected override void OnExecute()
    {
    }
}

internal class DotnetInstaller
{
    private static void DownloadSdkInstaller(string outputDir)
    {

    }
    
    public static void InstallSdk(string version, string outputDir)
    {

    }

    public static void InstallRuntime(string version,  string outputDir)
    {

    }

    public static void InstallWorkload(string version, string sdkDir)
    {
        
    }
}