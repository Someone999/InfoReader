using System;
using System.ComponentModel;
using System.Linq;

namespace InfoReaderPlugin.NewExpressionParser.Nodes
{
    public class OperatorExpressionNode : ExpressionNode
    {
        void CheckValueNode(params ValueExpressionNode[] nodes)
        {
            if (nodes.Any(node => node is null || node.Value is null))
                throw new ArgumentNullException();
        }
        public static int GetPriority(string op)
        {
            switch (op)
            {
                case "(":
                case ")": return 5;
                case "!": return 4;
                case "*":
                case "/": return 3;
                case "+":
                case "-": return 2;
                case "==":
                case ">=":
                case "<=":
                case ">":
                case "<":
                case "!=": return 1;
                case "&&":
                case "||": return 0;
                default: throw new ArgumentException($"Invalid operator {(string.IsNullOrEmpty(op) ? "(null or empty)" : op)}.");//Incorrect Operator.
            }
        }
        public OperatorExpressionNode(object objNode) : base(objNode)
        {
            Value = objNode;
        }
        public override ExpressionNodeType NodeType => ExpressionNodeType.Operator;
        int _priority = -1;
        public int Priority => _priority == -1 ? _priority = GetPriority(Value as string ?? throw new InvalidCastException()) : _priority;
        public ValueExpressionNode Calculate(ValueExpressionNode left, ValueExpressionNode right)
        {
            if (Value is string s)
            {
                switch (s)
                {
                    case "+": return Add((NumberExpressionNode)left, (NumberExpressionNode)right);
                    case "-": return Sub((NumberExpressionNode)left, (NumberExpressionNode)right);
                    case "*": return Mul((NumberExpressionNode)left, (NumberExpressionNode)right);
                    case "/": return Div((NumberExpressionNode)left, (NumberExpressionNode)right);
                    case ">=": return GreaterOrEquals((NumberExpressionNode)left, (NumberExpressionNode)right);
                    case "<=": return LessOrEquals((NumberExpressionNode)left, (NumberExpressionNode)right);
                    case ">": return Greater((NumberExpressionNode)left, (NumberExpressionNode)right);
                    case "<": return Less((NumberExpressionNode)left, (NumberExpressionNode)right);
                    case "==": return Equals(left, right);
                    case "!=": return NotEquals(left, right);
                    case "&&": return BoolAnd((BoolExpressionNode)left, (BoolExpressionNode)right);
                    case "||": return BoolOr((BoolExpressionNode)left, (BoolExpressionNode)right);
                    case "!": return Not((BoolExpressionNode)right);
                    default: throw new InvalidOperationException();
                }
            }
            throw new InvalidOperationException();
        }

        NumberExpressionNode Add(NumberExpressionNode a, NumberExpressionNode b)
        {
            CheckValueNode(a, b);
            return new NumberExpressionNode((double)a.Value + (double)b.Value);
        }

        NumberExpressionNode Sub(NumberExpressionNode a, NumberExpressionNode b)
        {
            CheckValueNode(a, b);
            return new NumberExpressionNode(-(double)a.Value + (double)b.Value);
        }

        NumberExpressionNode Mul(NumberExpressionNode a, NumberExpressionNode b)
        {
            CheckValueNode(a, b);
            return new NumberExpressionNode((double)a.Value * (double)b.Value);
        }

        NumberExpressionNode Div(NumberExpressionNode a, NumberExpressionNode b)
        {
            CheckValueNode(a, b);
            return new NumberExpressionNode((double)a.Value / (double)b.Value);
        }

        BoolExpressionNode Equals(ValueExpressionNode a, ValueExpressionNode b)
        {
            return new BoolExpressionNode(a.Equals(b));
        }
        BoolExpressionNode NotEquals(ValueExpressionNode a, ValueExpressionNode b)
        {
            return new BoolExpressionNode(!a.Equals(b));
        }

        BoolExpressionNode BoolAnd(BoolExpressionNode a, BoolExpressionNode b)
        {
            CheckValueNode(a, b);
            return new BoolExpressionNode((bool)a.Value && (bool)b.Value);
        }

        BoolExpressionNode BoolOr(BoolExpressionNode a, BoolExpressionNode b)
        {
            CheckValueNode(a, b);
            return new BoolExpressionNode((bool)a.Value || (bool)b.Value);
        }

        BoolExpressionNode Greater(NumberExpressionNode a, NumberExpressionNode b)
        {
            CheckValueNode(a, b);
            return new BoolExpressionNode((double)a.Value > (double)b.Value);
        }

        BoolExpressionNode Less(NumberExpressionNode a, NumberExpressionNode b)
        {
            CheckValueNode(a, b);
            return new BoolExpressionNode((double)a.Value < (double)b.Value);
        }

        BoolExpressionNode GreaterOrEquals(NumberExpressionNode a, NumberExpressionNode b)
        {
            CheckValueNode(a, b);
            return new BoolExpressionNode((double)a.Value >= (double)b.Value);
        }

        BoolExpressionNode LessOrEquals(NumberExpressionNode a, NumberExpressionNode b)
        {
            CheckValueNode(a, b);
            return new BoolExpressionNode((double)a.Value <= (double)b.Value);
        }

        BoolExpressionNode Not(BoolExpressionNode right)
        {
            CheckValueNode(right);
            return new BoolExpressionNode(!(bool)right.Value);
        }
    }

}