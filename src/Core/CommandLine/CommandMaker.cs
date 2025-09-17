using System.CommandLine;
using System.CommandLine.Parsing;
using System.Reflection;

namespace Zgo.Core;

internal interface IZgoCommand
{
    event Action<ParseResult> Parsers;
}
internal static class ZgoCommandMaker
{

    private static T GetTValueForOption<T>(this ParseResult result, Option<T> option)
    {
        return result.GetValue(option);
    }

    private static T GetTValueForArgument<T>(this ParseResult result, Argument<T> argument)
    {
        return result.GetValue(argument);
    }
    private static object GetValueForOption(this ParseResult result, Option option)
    {
        MethodInfo methodInfo = typeof(ZgoCommandMaker).GetMethod("GetTValueForOption", BindingFlags.NonPublic | BindingFlags.Static);
        MethodInfo getValueMethod = methodInfo.MakeGenericMethod(option.ValueType);
        return getValueMethod.Invoke(null, new object[] { result, option });
    }

    private static object GetValueForArgument(this ParseResult result, Argument argument)
    {
       MethodInfo methodInfo = typeof(ZgoCommandMaker).GetMethod("GetTValueForArgument", BindingFlags.NonPublic | BindingFlags.Static);
        MethodInfo getValueMethod = methodInfo.MakeGenericMethod(argument.ValueType);
        return getValueMethod.Invoke(null, new object[] { result, argument });
    }

    private static Func<ArgumentResult, T> GetFieldValueFactory<T>(this FieldInfo fieldInfo, IZgoCommand command)
    {
        return (reslult) =>
        {
            return (T)fieldInfo.GetValue(command);
        };
    }

    private static void SetDefaultValueFactoryForOption(this Option option, IZgoCommand command, FieldInfo fieldInfo)
    {
        PropertyInfo propertyInfo = option.GetType().GetProperty("DefaultValueFactory", BindingFlags.Instance | BindingFlags.Public);
        MethodInfo methodInfo = typeof(ZgoCommandMaker).GetMethod("GetFieldValueFactory", BindingFlags.NonPublic | BindingFlags.Static);
        MethodInfo getFieldValue = methodInfo.MakeGenericMethod(fieldInfo.FieldType);
        propertyInfo.SetValue(option, getFieldValue.Invoke(null, new object[] {fieldInfo, command}));
    }

    private static void SetDefaultValueFactoryForArgument(this Argument argument, IZgoCommand command, FieldInfo fieldInfo)
    {
        PropertyInfo propertyInfo = argument.GetType().GetProperty("DefaultValueFactory", BindingFlags.Instance | BindingFlags.Public);
        MethodInfo methodInfo = typeof(ZgoCommandMaker).GetMethod("GetFieldValueFactory", BindingFlags.NonPublic | BindingFlags.Static);
        MethodInfo getFieldValue = methodInfo.MakeGenericMethod(fieldInfo.FieldType);
        propertyInfo.SetValue(argument, getFieldValue.Invoke(null, new object[] {fieldInfo, command}));
    }
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
                Option option = null;
                if (optionAttr.Aliases != null && optionAttr.Aliases.Length > 0)
                {
                    option = Activator.CreateInstance(optionType, optionAttr.Name, optionAttr.Aliases) as Option;
                }
                else
                {
                    option = Activator.CreateInstance(optionType, optionAttr.Name) as Option;
                }
                SetOption(option, optionAttr);
                command.Options.Add(option);
                option.SetDefaultValueFactoryForOption(command, fieldInfo);
                command.Parsers += (result) =>
                {
                    object value = result.GetValueForOption(option);
                    fieldInfo.SetValue(command, value);
                };
            }
            else
            {
                Type argumentType = typeof(Argument<>).MakeGenericType(fieldType);
                Argument argument = Activator.CreateInstance(argumentType, argumentType.Name) as Argument;
                SetArgument(argument, arugmentAttr);
                command.Arguments.Add(argument);
                argument.SetDefaultValueFactoryForArgument(command, fieldInfo);
                command.Parsers += (result) =>
                {
                    object value = result.GetValueForArgument(argument);
                    fieldInfo.SetValue(command, value);
                };
            }
        }
    }

    private static void SetAllowValues(this Option option, string[] allowValues)
    {
        Type optionType = option.GetType();
        MethodInfo methodInfo = optionType.GetMethod("AcceptOnlyFromAmong");
        if (methodInfo != null)
        {
            methodInfo.Invoke(option, new object[] { allowValues });
        }
    }

    private static void SetAllowValues(this Argument argument, string[] allowValues)
    {
        Type argumentType = typeof(ArgumentValidation);
        MethodInfo methodInfo = argumentType.GetMethod("AcceptOnlyFromAmong").MakeGenericMethod(argument.ValueType);
        if (methodInfo != null)
        {
            methodInfo.Invoke(null, new object[] { argument, allowValues });
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
        option.SetAllowValues(optionAttr.AllowValues);
    }

    private static void SetArgument(Argument argument, ZgoCommand.ArgumentAttribute argumentAttr)
    {
        argument.Arity = argumentAttr.Arity;
        argument.Description = argumentAttr.Description;
        argument.HelpName = argumentAttr.HelpName;
        argument.SetAllowValues(argumentAttr.AllowValues);
    }



}