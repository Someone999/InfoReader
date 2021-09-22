namespace InfoReaderPlugin.ExpressionParser
{
    public sealed class IfExpression:Expression
    {
        public string TrueResult { get; set; }
        public string FalseResult { get; set; }
        public IfExpression(string expressionString,object target) :base(expressionString,target)
        {
        }
        public override ExpressionTypes Type => ExpressionTypes.If | ExpressionTypes.Builtin;
        public override object GetProcessedValue()
        {
            if (ExpressionString.StartsWith("$if{") && ExpressionString.EndsWith("}"))
            {
                int len = ExpressionString.Length - 4; 
                
               
                string[] args = ExpressionString.Remove(0, 4).Remove(len - 1, 1).Split(',');
                var rpnExp = ExpressionTools.ConvertToRpnExpression(args[0]);
                TrueResult = args[1];
                FalseResult = args[2];
                if (!TrueResult.StartsWith("\"") && !TrueResult.EndsWith("\""))
                {
                    TrueResult = ExpressionTools.CalcRpnExpression(ExpressionTools.ConvertToRpnExpression(TrueResult),Target);
                }
                else TrueResult = TrueResult.Trim('\"');
                if (!FalseResult.StartsWith("\"") && !FalseResult.EndsWith("\""))
                {
                    FalseResult = ExpressionTools.CalcRpnExpression(ExpressionTools.ConvertToRpnExpression(FalseResult), Target);
                }
                else FalseResult = FalseResult.Trim('\"');
                if (bool.Parse(ExpressionTools.CalcRpnExpression(rpnExp, Target)))
                    return TrueResult;
                return FalseResult;
            }
            else
            {
                return "Invalid Format! Please use $if{<condition>,<ifTrue>,<ifFalse>}";
            }
        }
    }
}