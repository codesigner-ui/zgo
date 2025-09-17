namespace Zgo.Core;

using System.CommandLine;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

public class ZgoCommand : Command, IZgoCommand
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OptionAttribute : ArgumentAttribute
    {
        public OptionAttribute(string name) : base(name)
        {
        }

        public bool Required { get; set; }
        public bool Recursive { get; set; }

        public string[] Aliases { get; set; } = new string[0];
        public bool Hidden { get; set; }
        public bool AllowMultipleArgumentsPerToken { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ArgumentAttribute : Attribute
    {
        public ArgumentArity Arity { get; set; }
        public string[] AllowValues { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string HelpName { get; set; }

        public ArgumentAttribute(string name)
        {
            this.Name = name;
        }

    }
    public ZgoCommand(string name, string description = "") : base(name, description)
    {
        ZgoCommandMaker.Make(this);
        MethodInfo methodInfo = this.GetType().GetMethod("OnExecute", BindingFlags.Instance | BindingFlags.NonPublic);

        bool executeAsync = methodInfo.DeclaringType != this.GetType();
        if (executeAsync)
            this.SetAction(this.OnActionAsync);
        else
            this.SetAction(this.OnAction);
    }

    public event Action<ParseResult> Parsers;

    public ZgoProgram RootProgram => Parents.First() as ZgoProgram;
    public ZgoCommand Parent => this.Parents.Last() as ZgoCommand;
    public T GetParent<T>() where T : ZgoCommand
    {
        foreach (var p in this.Parents)
        {
            if (p is T t)
            {
                return t;
            }
        }
        return null;
    }
    protected void OnParse(ParseResult result)
    {
        this.Parsers?.Invoke(result);
        this.PostParse();
    }
    protected void OnAction(ParseResult result)
    {
        OnParse(result);
        this.OnExecute();
    }

    protected async Task OnActionAsync(ParseResult result)
    {
        OnParse(result);
        await this.OnExecuteAsync();
    }

    protected virtual void PostParse()
    {
    }
    //
    protected virtual void OnExecute()
    {
    }

    protected virtual async Task OnExecuteAsync()
    {
    }
    
}
