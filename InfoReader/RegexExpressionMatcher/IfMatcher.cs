using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InfoReaderPlugin.RegexExpressionMatcher
{
    public class IfMatcher:ExpressionMatcherBase
    {
        public override Regex RegularExpression { get; protected set; } =
            new Regex(@"\$if\{[^\{\}]*[\w><=!&\|\+\-\*/\(\)]*,[^{}]*,[^{}]*\}");
    }
}