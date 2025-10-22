using Zgo.Core;
using Zgo.Project;
using Zgo.Toolchain;

public class MainProgram : ZgoProgram
{
    public MainProgram() : base("zgo project toolchain")
    {
        this.Add(new ProjectProgram());
        this.Add(new DotnetCommand());
        this.Add(new CMakeCommand());
    }

    public static void Main(string[] args)
    {
        MainProgram main = new MainProgram();
        main.Execute(args);
    }
}

