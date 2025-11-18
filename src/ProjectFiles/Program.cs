using Zgo.Core;

namespace Zgo.Project;

internal class ProjectGenerateCommand : ZgoCommand
{
    public ProjectGenerateCommand() : base("generate", "generate project files")
    {
    }
}

internal class ProjectNewCommand : ZgoCommand
{
    public ProjectNewCommand() : base("new", "new zgo project")
    {
    }
}

internal class ProjectSetupCommand : ZgoCommand
{
    public ProjectSetupCommand() : base("setup", "setup project toolchain")
    {
    }
}


public class ProjectProgram : ZgoCommand
{
    public ProjectProgram() : base("project", "zgo project")
    {
        this.Add(new ProjectNewCommand());
        this.Add(new ProjectGenerateCommand());
        this.Add(new ProjectSetupCommand());
    }
}
