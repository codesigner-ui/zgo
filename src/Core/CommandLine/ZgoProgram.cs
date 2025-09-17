
using System.CommandLine;

namespace Zgo.Core;

public class ZgoProgram : RootCommand, IZgoCommand
{
    public ParserConfiguration ParserConfiguration { get; set; } = new ParserConfiguration();
    public InvocationConfiguration InvocationConfiguration { get; set; } = new InvocationConfiguration();
    public ZgoProgram(string description = "") : base(description)
    {

    }

    public event Action<ParseResult> Parsers;

    public void Execute(string[] arguments)
    {
        ParseResult result = this.Parse(arguments, this.ParserConfiguration);
        result.Invoke(this.InvocationConfiguration);
    }
}