namespace InfoReaderPlugin.ExpressionParser
{
    public abstract class ExpressionType
    {
        public abstract string Type { get;}
        public abstract object GetProcessedValue();
        public virtual object Target { get; set; }
        public virtual string Expression { get; protected set; }

        protected ExpressionType(string expression,object target = null)
        {
            Expression = expression;
            Target = target;
        }
    }
}