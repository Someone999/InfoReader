using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InfoReaderPlugin.RegexExpressionMatcher
{
    public abstract class ExpressionMatcherBase
    {
        public virtual Regex RegularExpression { get; protected set; }

        public virtual void Match(string expression)
        {
            MatchCollection matches = RegularExpression.Matches(expression);
            if (matches.Count == 0)
            {
                Results = new string[0];
                return;
            }
            List<string> matchResults = new List<string>();
            foreach (Match match in matches)
            {
                matchResults.Add(match.Groups[0].Value);
            }
            Results = matchResults.ToArray();
        }
        public string[] Results { get; protected set; }
    }
}
