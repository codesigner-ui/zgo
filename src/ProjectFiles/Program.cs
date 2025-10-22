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


public class ProjectProgram : ZgoCommand
{
    public ProjectProgram() : base("project", "zgo project")
    {
        this.Add(new ProjectNewCommand());
        this.Add(new ProjectGenerateCommand());
    }
}
