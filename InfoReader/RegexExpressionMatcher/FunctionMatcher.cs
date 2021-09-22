using System.Collections.Generic;
using System.Text.RegularExpressions;
using osuTools.Attributes;

namespace InfoReaderPlugin.RegexExpressionMatcher
{
    [WorkingInProgress(DevelopmentStage.Stuck,"2021.9.22")]
    public class FunctionMatcher : ExpressionMatcherBase
    {
        public override Regex RegularExpression { get; protected set; } =
            new Regex(@"\$func\{\w+\([\w><=!&\|\+\-\*/\(\),\s]+\)(:*[\w\.]*)?\}");
    }
}