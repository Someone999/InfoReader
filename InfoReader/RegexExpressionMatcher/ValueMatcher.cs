using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InfoReaderPlugin.RegexExpressionMatcher
{
    public class ValueMatcher:ExpressionMatcherBase
    {
        public override Regex RegularExpression { get; protected set; } = new Regex(@"\$\{[^,\{\}]*(?::[\w\.]*)?\}");
    }
}
