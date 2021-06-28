namespace InfoReaderPlugin.ExpressionParser.Function
{
    public class FunctionBodyNode : FunctionNode
    {
        public FunctionBodyNode(string val) : base(val)
        {
        }
        public override FunctionNodeType NodeType => FunctionNodeType.Function;
    }
}