using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReaderPlugin.NewExpressionParser.Tools;
using Sync.Tools;

namespace InfoReaderPlugin.NewExpressionParser.Nodes
{
    public class IdentifierNode : ValueExpressionNode
    {
        static bool _notified = false;
        public IdentifierNode(object objNode) : base(objNode)
        {
            if (objNode is string s)
                Value = s;

        }

        public override ExpressionNodeType NodeType => ExpressionNodeType.Identifier;
        bool IsNumber(object obj)
        {
            return double.TryParse(obj.ToString(), out _);
        }
        bool IsBoolean(object obj) => bool.TryParse(obj.ToString(), out _);

        public ValueExpressionNode GetValue(object rootReflectObject)
        {
            if (Value is null)
                throw new ArgumentNullException();
            if (IsNumber(Value))
                return new NumberExpressionNode(Value);
            if (IsBoolean(Value))
                return new BoolExpressionNode(Value);
            VariablePropertyInfo info = null;
            if(InfoReader.VariablePropertyInfos.Any(i => (info = i).Alias == Value.ToString()))
            {
                /*if (!_notified && info.VariableName != Value.ToString())
                {
                    IO.CurrentIO.WriteColor($"You are using the unstandardized variable name (alias) {info.Alias} " +
                      $"(Standardized name is {info.VariableName}). I will drop unstandardized variable names (alias) in the next version. " +
                      $"You can get standardized names by executing \"getinfo list\"",
                      ConsoleColor.Red);
                    _notified = true;
                }*/

                Value = info.VariableName;
            }
            var obj = ValueGettingHelper.GetPropertyValue<object>(Value?.ToString() ?? throw new ArgumentNullException(), rootReflectObject);
            if (obj is null)
                throw new ArgumentNullException();
            if (IsNumber(obj))
                return new NumberExpressionNode(obj);
            if (IsBoolean(obj))
                return new BoolExpressionNode(obj);
            return new StringExpressionNode(obj);
        }
    }
}
