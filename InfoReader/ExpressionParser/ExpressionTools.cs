using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace InfoReaderPlugin.MemoryMapWriter.ExpressionTools
{
    static class ExpressionTools
    {

        public static Stack<string> ConvertToRpnExpression(string expression)
        {
            var boolOp = new Stack<string>();
            var objStack = new Stack<string>();
            var i = 0;
            var sb = new StringBuilder();
            while (i < expression.Length)
            {
                var cur = expression[i];
                
                if (cur == ' ')
                {
                    i++;
                    continue;
                }

                if (cur == '(')
                {
                    boolOp.Push(new string(new char[1] {cur}));
                    i++;
                    continue;
                }

                if (cur == ')')
                {
                    if(!string.IsNullOrEmpty(sb.ToString()))
                        objStack.Push(sb.ToString());
                    while (boolOp.Peek() != "(")
                    {
                        objStack.Push(boolOp.Peek());
                        boolOp.Pop();
                    }

                    if (boolOp.Count > 0)
                        boolOp.Pop();
                    sb.Clear();
                    i++;
                    continue;
                }
                if (!IsBoolOp(cur) && !IsCalcOp(cur))
                {
                    sb.Append(cur);
                }
                else
                {
                    if (!string.IsNullOrEmpty(sb.ToString()))
                        objStack.Push(sb.ToString());
                    if (IsBoolOp(expression[i + 1], out var nxtInputBoolChar))
                    {
                        var op = new string(new char[2] { cur, nxtInputBoolChar });
                        var priority = GetPriority(op);
                        if (boolOp.Count > 0)
                        {
                            var stackTopPriority = GetPriority(boolOp.Peek());
                            while (priority <= stackTopPriority && boolOp.Count > 0)
                            {
                                objStack.Push(boolOp.Pop());
                                if (boolOp.Count > 0) stackTopPriority = GetPriority(boolOp.Peek());
                            }
                        }

                        i++;
                        boolOp.Push(op);
                        sb.Clear();
                    }
                    else if (IsCalcOp(cur))
                    {
                        var calcOp = new string(new[] { cur });
                        var calcPriority = GetPriority(calcOp);
                        if (boolOp.Count > 0 && boolOp.Peek()!="(")
                        {
                            var stackTopPriority = GetPriority(boolOp.Peek());
                            while (calcPriority <= stackTopPriority && boolOp.Count > 0 && boolOp.Peek() != "(")
                            {
                                objStack.Push(boolOp.Pop());
                                if (boolOp.Count > 0) stackTopPriority = GetPriority(boolOp.Peek());
                            }
                        }
                        boolOp.Push(calcOp);
                        sb.Clear();
                    }
                    else
                    {
                        var op = new string(new[] { cur });
                        var priority = GetPriority(op);
                        if (boolOp.Count > 0 && boolOp.Peek() != "(")
                        {
                            var stackTopPriority = GetPriority(boolOp.Peek());
                            while (priority <= stackTopPriority && boolOp.Count > 0 && boolOp.Peek() != "(")
                            {
                                objStack.Push(boolOp.Pop());
                                if (boolOp.Count > 0) stackTopPriority = GetPriority(boolOp.Peek());
                            }
                        }
                        boolOp.Push(op);
                        sb.Clear();
                    }
                }
                i++;
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
                objStack.Push(sb.ToString());
            while (boolOp.Count > 0)
                objStack.Push(boolOp.Pop());
            sb.Clear();
            return objStack;
        }
        public static string CalcRpnExpression(Stack<string> calcStack, object reflectObject)
        {
            try
            {



                if (calcStack == null || calcStack.Count == 0)
                    return "false";
                var obj = new Stack<string>();
                var op = new Stack<string>();
                var rslt = new Stack<string>(calcStack);
                while (true)
                {
                    string cur = "";
                    if (rslt.Count > 0)
                        cur = rslt.Pop();
                    if (!IsBoolOp(cur) && !IsCalcOp(cur) && !string.IsNullOrEmpty(cur))
                    {
                        obj.Push(cur);
                    }
                    else
                    {
                        if (IsBoolOp(cur))
                        {
                            if (cur == "==")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = !IsVariable<object>(bStr, reflectObject, out var bValue)
                                    ? bStr
                                    : bValue.ToString();
                                var a = !IsVariable<object>(aStr, reflectObject, out var aValue)
                                    ? aStr
                                    : aValue.ToString();
                                obj.Push((a.Equals(b)).ToString());
                                continue;
                            }

                            if (cur == "!=")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = !IsVariable<object>(bStr, reflectObject, out var bValue)
                                    ? bStr
                                    : bValue.ToString();
                                var a = !IsVariable<object>(aStr, reflectObject, out var aValue)
                                    ? aStr
                                    : aValue.ToString();
                                obj.Push((!a.Equals(b)).ToString());
                                continue;
                            }

                            if (cur == "&&")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = !IsVariable<object>(bStr, reflectObject, out var bValue) ? bStr : bValue;
                                var a = !IsVariable<object>(aStr, reflectObject, out var aValue) ? aStr : aValue;
                                obj.Push(((bool) Convert.ChangeType(a, typeof(bool)) &&
                                          (bool) Convert.ChangeType(b, typeof(bool))).ToString());
                                continue;
                            }

                            if (cur == ">=")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = IsVariable<string>(bStr, reflectObject, out var bValue)
                                    ? double.Parse(bValue)
                                    : double.Parse(bStr);
                                var a = IsVariable<string>(aStr, reflectObject, out var aValue)
                                    ? double.Parse(aValue)
                                    : double.Parse(aStr);
                                obj.Push((a >= b).ToString());
                                continue;
                            }

                            if (cur == "<=")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = IsVariable<string>(bStr, reflectObject, out var bValue)
                                    ? double.Parse(bValue)
                                    : double.Parse(bStr);
                                var a = IsVariable<string>(aStr, reflectObject, out var aValue)
                                    ? double.Parse(aValue)
                                    : double.Parse(aStr);
                                obj.Push((a <= b).ToString());
                                continue;
                            }

                            if (cur == ">")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = IsVariable<string>(bStr, reflectObject, out var bValue)
                                    ? double.Parse(bValue)
                                    : double.Parse(bStr);
                                var a = IsVariable<string>(aStr, reflectObject, out var aValue)
                                    ? double.Parse(aValue)
                                    : double.Parse(aStr);
                                obj.Push((a > b).ToString());
                                continue;
                            }

                            if (cur == "<")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = IsVariable<string>(bStr, reflectObject, out var bValue)
                                    ? double.Parse(bValue)
                                    : double.Parse(bStr);
                                var a = IsVariable<string>(aStr, reflectObject, out var aValue)
                                    ? double.Parse(aValue)
                                    : double.Parse(aStr);
                                obj.Push((a < b).ToString());
                                continue;
                            }

                            if (cur == "||")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = !IsVariable<object>(bStr, reflectObject, out var bValue) ? bStr : bValue;
                                var a = !IsVariable<object>(aStr, reflectObject, out var aValue) ? aStr : aValue;
                                obj.Push(((bool) Convert.ChangeType(a, typeof(bool)) ||
                                          (bool) Convert.ChangeType(b, typeof(bool))).ToString());
                                continue;
                            }

                            if (cur == "!")
                            {
                                string bStr = obj.Pop();
                                var b = !IsVariable<object>(bStr, reflectObject, out var bValue) ? bStr : bValue;
                                obj.Push((!(bool) Convert.ChangeType(b, typeof(bool))).ToString());
                                continue;
                            }
                        }

                        if (IsCalcOp(cur))
                        {
                            if (cur == "+")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = IsVariable<string>(bStr, reflectObject, out var bValue)
                                    ? double.Parse(bValue)
                                    : double.Parse(bStr);
                                var a = IsVariable<string>(aStr, reflectObject, out var aValue)
                                    ? double.Parse(aValue)
                                    : double.Parse(aStr);
                                obj.Push((a + b).ToString(CultureInfo.InvariantCulture));
                                continue;
                            }

                            if (cur == "-")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = IsVariable<string>(bStr, reflectObject, out var bValue)
                                    ? double.Parse(bValue)
                                    : double.Parse(bStr);
                                var a = IsVariable<string>(aStr, reflectObject, out var aValue)
                                    ? double.Parse(aValue)
                                    : double.Parse(aStr);
                                obj.Push((a - b).ToString(CultureInfo.InvariantCulture));
                                continue;
                            }

                            if (cur == "*")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = IsVariable<string>(bStr, reflectObject, out var bValue)
                                    ? double.Parse(bValue)
                                    : double.Parse(bStr);
                                var a = IsVariable<string>(aStr, reflectObject, out var aValue)
                                    ? double.Parse(aValue)
                                    : double.Parse(aStr);
                                obj.Push((a * b).ToString(CultureInfo.InvariantCulture));
                            }

                            if (cur == "/")
                            {
                                string bStr = obj.Pop();
                                string aStr = obj.Pop();
                                var b = IsVariable<string>(bStr, reflectObject, out var bValue)
                                    ? double.Parse(bValue)
                                    : double.Parse(bStr);
                                var a = IsVariable<string>(aStr, reflectObject, out var aValue)
                                    ? double.Parse(aValue)
                                    : double.Parse(aStr);
                                obj.Push((a / b).ToString(CultureInfo.InvariantCulture));
                                continue;
                            }
                        }
                    }

                    if (rslt.Count <= 0)
                    {
                        if (obj.Count == 1)
                        {
                            if (IsVariable<object>(obj.Peek(), reflectObject, out var casted))
                                return casted.ToString();
                        }

                        if (obj.Count > 1)
                            throw new InvalidOperationException();
                        return obj.Peek();
                    }
                }
            }
            catch (FormatException)
            {
                return "False";
            }
        }
        public static bool IsBoolOp(char c)
        {
            return c == '&' || c == '|' || c == '!' || c == '=' || c == '>' || c == '<';
        }
        public static bool IsBoolOp(char c, out char inputChar)
        {
            inputChar = c;
            return c == '&' || c == '|' || c == '!' || c == '=' || c == '>' || c == '<';
        }
        public static bool IsBoolOp(string s)
        {
            return s == "&&" || s == "||" || s == "!=" || s == "==" || s == ">=" || s == "<=" || s == "<" ||
                   s == ">" || s == "!";
        }
        public static bool IsCalcOp(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/';
        }
        public static bool IsCalcOp(string s)
        {
            return s == "+" || s == "-" || s == "*" || s == "/";
        }
        public static bool IsCalcOp(char c, out char inputChar)
        {
            inputChar = c;
            return c == '+' || c == '-' || c == '*' || c == '/';
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
                default: throw new ArgumentException("不正确的运算符。");//Incorrect Operator.
            }
        }
        public static bool IsVariable<T>(string expression, object target, out T castedObject)
        {
            object val = new VariableExpression(expression, target).GetProcessedValue();
            castedObject = default;
            if(val is IConvertible)
               castedObject = (T)Convert.ChangeType(val,typeof(T));
            if(typeof(T)==typeof(object))
               castedObject = (T)val;
            return val.ToString() != "Unknown Variable";
        }

        
    }
}