using InfoReaderPlugin.NewExpressionParser.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace InfoReaderPlugin.NewExpressionParser.Tools
{
    public static class RpnTools
    {
        public static bool IsOperator(char token)
        {
            char[] ops = { '(', ')', '!', '*', '/', '+', '-', '=', '>', '<', '&', '|' };
            return ops.Contains(token);
        }

        public static bool IsTwoCharacterOperator(char lastToken, char currentToken)
        {
            char[] subOps = { '=', '&', '|' };
            if (IsOperator(lastToken))
            {
                return subOps.Contains(currentToken);
            }
            return false;
        }
        //后缀表达式处理运算符优先级
        /*
         * 用例：
         * 1+5*3*(1+2)
         * 变换方法
         * 1 5 3 1 2 + * * +
         * + *(2) *(1) ( + )
         * 
         * 运算方法
         * 1 5 3 1 2
         * + * * +
         * 1 + 2 = 3  | 1 5 3 3
         *            | + * *
         * 3 * 3 = 9  | 1 5 9
         *            | + *
         * 9 * 5 = 45 | 1 45
         *            | +
         * 1 + 45 = 46 即为结果
         */
        internal static void PriorityProcessor(Stack<OperatorExpressionNode> opNodes, Stack<ExpressionNode> nodes, OperatorExpressionNode op)
        {
            if (op is null || op.Value is null)
                throw new ArgumentNullException();
            if (op.Value.Equals("("))
            {
                opNodes.Push(op);
                return;
            }
            if (op.Value.Equals(")"))
            {
                while (opNodes.Count > 0)
                {
                    var p = opNodes.Pop();
                    if (p is null || p.Value is null)
                        throw new ArgumentNullException();
                    if (!p.Value.Equals("("))
                    {
                        nodes.Push(p);
                    }
                }
                return;
            }
            while (opNodes.Count > 0 && op.Priority <= opNodes.Peek().Priority && !(opNodes.Peek().Value?.Equals("(") ?? throw new ArgumentNullException()))
            {
                nodes.Push(opNodes.Pop());
                if (opNodes.Count == 0)
                {
                    opNodes.Push(op);
                    return;
                }
            }
            opNodes.Push(op);
        }

        public static Stack<ExpressionNode> ToRpnExpression(string expression)
        {
            
            Stack<ExpressionNode> nodes = new Stack<ExpressionNode>();
            Stack<OperatorExpressionNode> opNodes = new Stack<OperatorExpressionNode>();
            StringBuilder currentToken = new StringBuilder();
            int lastIndex = -1;
            for (int i = 0; i < expression.Length;)
            {
                
                if (char.IsWhiteSpace(expression[i]))
                {                    
                    i++;
                    continue;
                }

                if (char.IsDigit(expression[i]))
                {
                    if (currentToken.Length > 0 && !double.TryParse(currentToken.ToString(), out _))
                    {                      
                        currentToken.Append(expression[i++]);
                        continue;
                    }
                    while ((char.IsDigit(expression[i]) || expression[i] == '.') && i < expression.Length)
                    {
                        currentToken.Append(expression[i]);
                        if (i < expression.Length - 1)
                        {                          
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    string lastToken = currentToken.ToString();
                    if (double.TryParse(lastToken, out double result))
                    {
                        nodes.Push(new NumberExpressionNode(result));
                    }
                    currentToken.Clear();
                    //Ver1 i > expression.Length - 1
                    //Ver2 i > expression.Length
                    if (i == lastIndex || lastIndex == expression.Length - 1)
                        break;
                }
                if (IsOperator(expression[i]))
                {
                    if (currentToken.Length > 0)
                    {
                        if (expression[i] == '(')
                        {
                            while (expression[i] != ')')
                            {
                                currentToken.Append(expression[i]);
                                if (i >= expression.Length - 1)
                                    break;
                                i++;
                            }
                            FunctionParser(nodes, currentToken.ToString());
                            currentToken.Clear();
                            continue;
                        }
                        string lastToken = currentToken.ToString();
                        nodes.Push(new IdentifierNode(lastToken));
                    }
                    currentToken.Clear();
                    currentToken.Append(expression[i]);
                    if (i < expression.Length)
                    {
                        i++;
                    }
                    else if (currentToken.ToString() != ")")
                    {
                        throw new ArgumentException("Operator at end of expression");
                    }
                    if (i < expression.Length - 1)
                    {
                        if (IsTwoCharacterOperator(expression[i - 1], expression[i]))
                        {
                            currentToken.Append(expression[i]);
                            i++;
                        }
                    }
                    string op = currentToken.ToString();
                    OperatorExpressionNode opNode = new OperatorExpressionNode(op);
                    PriorityProcessor(opNodes, nodes, opNode);
                    currentToken.Clear();
                    if (i > expression.Length - 1)
                        break;

                    continue;
                }
                currentToken.Append(expression[i]);
                lastIndex = i;
                i++;

            }

            if (currentToken.Length > 0)
            {
                if (currentToken.Length == 1)
                {
                    if (IsOperator(currentToken[0]))
                        throw new ArgumentException("Operator at last place.");
                }
                else if (currentToken.Length == 2)
                {
                    if (IsTwoCharacterOperator(currentToken[0], currentToken[1]))
                        throw new ArgumentException("Operator at last place.");
                }
                if (double.TryParse(currentToken.ToString(), out var rslt))
                {
                    nodes.Push(new NumberExpressionNode(rslt));
                }
                else
                {
                    nodes.Push(new IdentifierNode(currentToken.ToString()));
                }
            }

            while (opNodes.Count > 0)
            {
                nodes.Push(opNodes.Pop());
            }
             
            return nodes;
        }

        public static ExpressionNode CalcRpnStack(Stack<ExpressionNode> nodes, object valueReflectObject)
        {
            nodes = new Stack<ExpressionNode>(nodes);
            Stack<ValueExpressionNode> vals = new Stack<ValueExpressionNode>();

            while (nodes.Count > 0)
            {
                if (nodes.Peek() is ValueExpressionNode)
                {
                    var val = nodes.Pop() as ValueExpressionNode;
                    if (val is null)
                        throw new Exception("Please contact the developer.");
                    if (val is IdentifierNode i)
                    {
                        val = i.GetValue(valueReflectObject);
                    }
                    vals.Push(val);
                    continue;
                }
                if (nodes.Peek() is OperatorExpressionNode op)
                {
                    if (op.Value.Equals("!"))
                    {
                        vals.Push(op.Calculate(null, vals.Pop()));
                    }
                    else
                    {
                        var val2 = vals.Pop();
                        var val1 = vals.Pop();
                        vals.Push(op.Calculate(val1, val2));
                    }
                    nodes.Pop();
                    continue;
                }

                if (nodes.Peek() is FunctionArgumentNode)
                {
                    var func = nodes.Pop() as FunctionArgumentNode;
                    while (nodes.Peek() is FunctionArgumentNode arg)
                    {
                        nodes.Pop();
                    }
                    continue;
                }
            }
            if (vals.Count > 1)
                throw new Exception("Rpn expression is incorrect");
            return vals.Pop();
        }


        public static void FunctionParser(Stack<ExpressionNode> nodeStack, string expression)
        {
            StringBuilder currentToken = new StringBuilder();
            Stack<ExpressionNode> nodes = new Stack<ExpressionNode>();
            int layer = 0;
            bool inFunction = false;
            for (int i = 0; i < expression.Length; i++)
            {

                if (expression[i] == '(')
                {
                    if (currentToken.Length > 0)
                    {
                        FunctionNode func = new FunctionNode(currentToken.ToString());
                        nodeStack.Push(func);
                        inFunction = true;
                        layer++;
                        currentToken.Clear();
                    }
                    else
                    {
                        layer++;
                    }
                    continue;
                }

                if (expression[i] == ')')
                {
                    layer--;
                    if (layer > 0)
                    {
                        inFunction = true;
                        if (currentToken.Length > 0)
                            nodeStack.Push(new FunctionArgumentNode(currentToken.ToString()));
                    }

                    continue;
                }

                if (expression[i] == ',')
                {
                    nodeStack.Push(new FunctionArgumentNode(currentToken.ToString()));
                    currentToken.Clear();
                    continue;
                }

                if (char.IsDigit(expression[i]))
                {
                    while (char.IsDigit(expression[i]))
                    {
                        currentToken.Append(expression[i]);
                        if (i >= expression.Length - 1)
                            break;
                    }
                    nodeStack.Push(new NumberExpressionNode(currentToken.ToString()));
                    continue;
                }


                currentToken.Append(expression[i]);
            }

        }
    }
}
