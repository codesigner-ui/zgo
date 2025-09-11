
using System.CommandLine;

namespace Zgo.Core;

public class ZgoProgram : RootCommand, IZgoCommand
{
    public ZgoProgram(string description = "") : base(description)
    {

    }
}