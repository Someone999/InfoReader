using System;

namespace InfoReaderPlugin.ExpressionParser.Function
{
    public class ArgumentNode : FunctionNode
    {
        public ArgumentNode(string val) : base(val)
        {
        }
        public override FunctionNodeType NodeType => FunctionNodeType.Argument;

        public string[] SplitedArgument => Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

    }
}