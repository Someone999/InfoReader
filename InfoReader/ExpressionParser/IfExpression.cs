namespace InfoReaderPlugin.ExpressionParser
{
    public sealed class IfExpression:ExpressionType
    {
        public string TrueResult { get; set; }
        public string FalseResult { get; set; }
        public IfExpression(string expression,object target) :base(expression,target)
        {
        }
        public override string Type => "If to judgement";
        public override object GetProcessedValue()
        {
            if (Expression.StartsWith("$if{") && Expression.EndsWith("}"))
            {
                int len = Expression.Length - 4; 
                
               
                string[] args = Expression.Remove(0, 4).Remove(len - 1, 1).Split(',');
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