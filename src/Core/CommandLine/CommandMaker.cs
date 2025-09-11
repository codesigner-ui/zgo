using System.CommandLine;
using System.Reflection;

namespace Zgo.Core;

internal interface IZgoCommand
{

}
internal static class ZgoCommandMaker
{
    public static void Make<T>(T command) where T : Command, IZgoCommand
    {
        Type commandType = command.GetType();
        Type thisType = commandType;
        while (typeof(IZgoCommand).IsAssignableFrom(thisType))
        {
            MakeCommandHelper(thisType, command);
        }
    }

    private static void MakeCommandHelper(Type commandType, IZgoCommand zgoCommand)
    {

    }

    private static void MakeArgument(Command command, MemberInfo memberInfo)
    {

    }

    private static void MakeOption(Command command, MemberInfo memberInfo)
    {

    }

}