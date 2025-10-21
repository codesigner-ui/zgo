namespace Zgo.Toolchain;

using Zgo.Core;
public class DotnetCommand : ZgoCommand
{
    public DotnetCommand() : base("dotnet", "dotnet toolchain")
    {
        this.Add(new DotnetInstallCommand());
        this.Add(new PackageDownloadCommand());
        this.Add(new DotnetExecCommand());
    }
}