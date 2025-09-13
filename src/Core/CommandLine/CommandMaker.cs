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
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (FieldInfo fieldInfo in thisType.GetFields(bindingFlags))
        {
            ZgoCommand.ArgumentAttribute arugmentAttr = fieldInfo.GetCustomAttribute<ZgoCommand.ArgumentAttribute>();
            if (arugmentAttr == null)
            {
                continue;
            }
            Type fieldType = fieldInfo.FieldType;

            object defaultValue = fieldInfo.GetValue(command);
            if (arugmentAttr is ZgoCommand.OptionAttribute optionAttr)
            {
                Type optionType = typeof(Option<>).MakeGenericType(fieldType);
                Option option = Activator.CreateInstance(optionType, optionAttr.Name, optionAttr.Aliases) as Option;
                SetOption(option, optionAttr);
                command.Options.Add(option);
                
            }
            else
            {
                Type argumentType = typeof(Argument<>).MakeGenericType(fieldType);
                Argument argument = Activator.CreateInstance(argumentType, argumentType.Name) as Argument;
                SetArgument(argument, arugmentAttr);
                command.Arguments.Add(argument);
            }
        }
    }

    private static void SetOption(Option option, ZgoCommand.OptionAttribute optionAttr)
    {
        option.Description = optionAttr.Description;
        option.HelpName = optionAttr.HelpName;
        option.Arity = optionAttr.Arity;
        option.Recursive = optionAttr.Recursive;
        option.AllowMultipleArgumentsPerToken = optionAttr.AllowMultipleArgumentsPerToken;
        option.Hidden = optionAttr.Hidden;
        option.Required = optionAttr.Required;
    }

    private static void SetArgument(Argument argument, ZgoCommand.ArgumentAttribute argumentAttr)
    {
        argument.Arity = argumentAttr.Arity;
        argument.Description = argumentAttr.Description;
        argument.HelpName = argumentAttr.HelpName;
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