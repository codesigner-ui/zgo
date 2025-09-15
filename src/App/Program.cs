using Zgo.Core;
using Zgo.Toolchain;

public class MainProgram : ZgoProgram
{
    public MainProgram() : base("zgo project toolchain")
    {
        this.Add(new DotnetCommand());
    }

    public static void Main(string[] args)
    {
        MainProgram main = new MainProgram();
        main.Execute(args);
    }
}

