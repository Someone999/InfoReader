using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace InfoReaderPlugin.NewExpressionParser.Nodes
{
    public abstract class ExpressionNode
    {
        protected ExpressionNode(object objNode)
        {
            if (objNode is null)
                throw new ArgumentNullException(nameof(objNode));
        }
        public abstract ExpressionNodeType NodeType { get; }
        public virtual object Value { get; protected set; } = string.Empty;
        public virtual string ToEquableString() => Value?.ToString();
        public override string ToString()
        {
            return Value?.ToString();
        }

        public string ToString(string format)
        {
            Type t = Value?.GetType();
            MethodInfo methodInfo = t.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
            if (methodInfo is null)
            {
                return ToString();
                //throw new MissingMethodException($"Can not find  \"ToString\" with a string argument in type {t}");
            }

            return methodInfo.Invoke(Value, new object[] { format }) as string ?? ToString();
            //throw new MissingMethodException($"Can not find  \"ToString\" with a string argument in type {t}")
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;
            if (obj is ExpressionNode node)
            {
                if (node.Value is ExpressionNode n)
                    throw new InvalidOperationException();
                if (node.Value is null && Value is null)
                    return true;
                if (node.Value is object o)
                    return o.Equals(Value);
                return false;
            }
            return false;
        }
        public static bool operator ==(ExpressionNode a, ExpressionNode b) => a.Equals(b);
        public static bool operator !=(ExpressionNode a, ExpressionNode b) => !a.Equals(b);

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }

}