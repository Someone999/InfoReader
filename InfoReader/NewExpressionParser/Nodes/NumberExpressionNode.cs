using System;

namespace InfoReaderPlugin.NewExpressionParser.Nodes
{
    public class NumberExpressionNode : ValueExpressionNode
    {
        public NumberExpressionNode(object objNode) : base(objNode)
        {
            Value = Convert.ToDouble(objNode);
        }
        public override ExpressionNodeType NodeType => ExpressionNodeType.Number;

    }

}