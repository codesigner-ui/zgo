namespace Zgo.Core;

using System.CommandLine;

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
    public ZgoCommand(string name, string description="") : base(name, description)
    {
        ZgoCommandMaker.Make(this);
        this.SetAction(this.OnAction);
    }

    public event Action<ParseResult> OnParser;

    protected virtual void OnAction(ParseResult result)
    {
        this.OnParser?.Invoke(result);
        this.PostParse();
        this.OnExecute();
    }

    protected virtual void PostParse()
    {

    }
    //
    protected virtual void OnExecute()
    {

    }
    
}
