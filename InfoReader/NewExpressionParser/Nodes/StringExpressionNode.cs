using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReaderPlugin.NewExpressionParser.Nodes
{
    public class StringExpressionNode : ValueExpressionNode
    {
        public StringExpressionNode(object objNode) : base(objNode)
        {
            Value = objNode.ToString() ?? string.Empty;
        }

        public override ExpressionNodeType NodeType => ExpressionNodeType.String;
    }
}
