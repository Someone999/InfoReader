namespace InfoReaderPlugin.ExpressionParser.Function
{
    public abstract class FunctionNode
    {
        protected FunctionNode(string val)
        {
            Value = val;
        }

        public virtual FunctionNodeType NodeType { get; } = FunctionNodeType.None;
        public string Value { get; set; }
    }
}
