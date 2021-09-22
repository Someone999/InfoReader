namespace InfoReaderPlugin.ExpressionParser
{
    public abstract class Expression
    {
        public abstract ExpressionTypes Type { get;}
        public abstract object GetProcessedValue();
        public virtual object Target { get; set; }
        public virtual string ExpressionString { get; protected set; }

        protected Expression(string expressionString,object target = null)
        {
            ExpressionString = expressionString;
            Target = target;
        }
    }
}