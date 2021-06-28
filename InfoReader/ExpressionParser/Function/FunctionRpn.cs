using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace InfoReaderPlugin.ExpressionParser.Function
{
    public static class FunctionRpn
    {
        public static int GetCharNum(string str, char c)
        {
            int count = 0;
            foreach (char ch in str)
                count += c == ch ? 1 : 0;
            return count;
        }
        public static Stack<FunctionNode> CalcRpn(string s)
        {
            int left = GetCharNum(s, '(');
            int right = GetCharNum(s, ')');
            if (left != right)
            {
                StringBuilder exp = new StringBuilder("\n括号不匹配\n");
                var index = s.LastIndexOf(left > right ? '(' : ')');
                exp.AppendLine(s);
                for (int i = 0; i < index + 3; i++)
                    exp.Append(" ");
                exp.Append("^");
                throw new ArgumentException(exp.ToString());
            }
            Stack<FunctionNode> func = new Stack<FunctionNode>();
            StringBuilder builder = new StringBuilder();
            foreach (var c in s)
            {

                if (c == '(')
                {
                    func.Push(new FunctionBodyNode(builder.ToString()));
                    builder.Clear();
                }

                else if (c == ')')
                {
                    string args = builder.ToString();
                    builder.Clear();
                    if (!string.IsNullOrEmpty(args))
                        func.Push(new ArgumentNode(args));
                }
                else if (c == ',')
                {
                    if (!string.IsNullOrEmpty(builder.ToString()))
                    {
                        func.Push(new CalcNode(builder.ToString()));
                        builder.Clear();
                    }
                }
                else
                {
                    builder.Append(c);
                }
            }
            return func;
        }

        public static string InvokeFunctionRpn(Stack<FunctionNode> funcAndArgs, TypeConstructorInfo methodTypeConstructorInfo,TypeConstructorInfo argsTypeConstructorInfo = null)
        {
            Stack<object> args = new Stack<object>();
            if (!methodTypeConstructorInfo.IsStatic)
            {
                if (methodTypeConstructorInfo.ExistedValue is null)
                    methodTypeConstructorInfo.CreateInstance(true);
                if (argsTypeConstructorInfo is null)
                    argsTypeConstructorInfo = methodTypeConstructorInfo;
            }
            while (funcAndArgs.Count > 0)
            {
                FunctionNode node = funcAndArgs.Pop();
                if (node is ArgumentNode argumentNode)
                {
                    string[] arguments = argumentNode.SplitedArgument;
                    foreach (var argument in arguments)
                    {
                        var rpnExpression = ExpressionTools.ConvertToRpnExpression(argument);
                        object existedArgsTypeObject = argsTypeConstructorInfo?.ExistedValue ?? argsTypeConstructorInfo?.CreateInstance();
                        
                        args.Push(ExpressionTools.CalcRpnExpression(rpnExpression, existedArgsTypeObject));
                    }
                }
                else if (node is FunctionBodyNode functionBody)
                {
                    Type t = methodTypeConstructorInfo.TargeType;
                    MethodInfo reflectMethod = t.GetMethod(functionBody.Value);
                    if (reflectMethod is null)
                        throw new MissingMethodException("找不到指定的方法");
                    int paramsCount = reflectMethod.GetParameters().Length;
                    List<object> invokeArgs = new List<object>();
                    for (int i = 0; i < paramsCount; i++)
                    {
                        invokeArgs.Add(args.Pop());
                    }
                    args.Push(reflectMethod.Invoke(methodTypeConstructorInfo.ExistedValue, invokeArgs.ToArray()));
                }
            }
            return args.Pop().ToString();
        }
    }
}