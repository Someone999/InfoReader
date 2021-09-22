using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReaderPlugin.ExpressionParser
{
    [Flags]
    public enum ExpressionTypes
    {
        Builtin = 1024,
        Variable = 1,
        If = 2,
        Function = 4
    }
}
