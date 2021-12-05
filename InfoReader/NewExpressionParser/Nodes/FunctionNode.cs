using InfoReaderPlugin.NewExpressionParser.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InfoReaderPlugin.NewExpressionParser.Nodes
{
    public class FunctionNode : ExpressionNode
    {
        public FunctionNode(object objNode) : base(objNode)
        {
            if (objNode is string)
            {
                Value = objNode;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        MethodInfo _internalMethod;

        private bool TryGetMethod(string name, object reflectObject, out MethodInfo method,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            method = reflectObject?.GetType().GetMethod(name, bindingFlags);
            if (method == null)
            {
                return false;
            }
            else
            {
                _internalMethod = method;
                return true;
            }

        }

        public ValueExpressionNode Invoke(object rootReflectObject)
        {
            return null;

        }

        public override ExpressionNodeType NodeType => ExpressionNodeType.Function;
    }

    public class FunctionArgumentNode : ValueExpressionNode
    {
        public FunctionArgumentNode(object objNode) : base(objNode)
        {
            Value = objNode;
        }
        public ValueExpressionNode Calculate(object rootReflectObject)
        {
            if (Value is string s)
            {
                var rpnExp = RpnTools.ToRpnExpression(s);
                var rslt = RpnTools.CalcRpnStack(rpnExp, rootReflectObject);
                if (double.TryParse(rslt.ToString(), out _))
                {
                    return new NumberExpressionNode(rslt);
                }
                if (bool.TryParse(rslt.ToString(), out _))
                {
                    return new BoolExpressionNode(rslt);
                }
                return new StringExpressionNode(rslt);
            }
            if (Value is null)
                return null;
            if (double.TryParse(Value.ToString(), out _))
            {
                return new NumberExpressionNode(Value);
            }
            if (bool.TryParse(Value.ToString(), out _))
            {
                return new BoolExpressionNode(Value);
            }
            return new StringExpressionNode(Value);
        }
        public override ExpressionNodeType NodeType => ExpressionNodeType.Argument;
    }
}
