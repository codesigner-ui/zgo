namespace Zgo.Core;

using System.CommandLine;

public class ZgoCommand : Command
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ArgumentAttribute : Attribute
    {

    }
    public ZgoCommand(string name) : base(name)
    {

    }
}
