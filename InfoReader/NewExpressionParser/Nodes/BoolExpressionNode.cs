using System;

namespace InfoReaderPlugin.NewExpressionParser.Nodes
{
    public class BoolExpressionNode : ValueExpressionNode
    {
        public BoolExpressionNode(object objNode) : base(objNode)
        {
            Value = Convert.ToBoolean(objNode);
        }

        public override ExpressionNodeType NodeType => ExpressionNodeType.Bool;
    }

}