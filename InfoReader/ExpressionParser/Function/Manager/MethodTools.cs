using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InfoReaderPlugin.Exceptions;

namespace InfoReaderPlugin.ExpressionParser.Function.Manager
{
    public static class MethodTools
    {
        public static bool CheckParameterTypes(Type[] invokeTypes, Type[] paramList, bool throwWhenFailed)
        {
            if (invokeTypes.Length == 0 && paramList.Length == 0)
                return true;
            if (invokeTypes.Length == 0 || paramList.Length == 0)
                return false;
            for (int i = 0; i < paramList.Length; i++)
            {
                if (paramList[i] != invokeTypes[i])
                {
                    StringBuilder currentTypes = new StringBuilder("("), correctTypes = new StringBuilder("(");
                    for (int j = 0; j < paramList.Length; j++)
                    {
                        Type currentType = invokeTypes[j], correctType = paramList[j];
                        correctTypes.Append(correctType.Name);
                        currentTypes.Append(currentType.Name);
                        if (j < paramList.Length - 1)
                        {
                            correctTypes.Append(",");
                            currentTypes.Append(",");
                        }
                    }
                    correctTypes.Append(')');
                    currentTypes.Append(')');
                    if (throwWhenFailed)
                        throw new InvokeFailedException("调用指定方法失败",new ParameterTypesMismatchedException($"参数类型不匹配。应为{correctTypes}，但是现在是{currentTypes}"));
                    return false;
                }
            }
            return true;
        }

        public static bool CheckParameterCount(int invokeParamCount, int paramCount, bool throwWhenFailed)
        {
            if (invokeParamCount == paramCount) return true;
            if (throwWhenFailed)
            {
                throw new InvokeFailedException("调用指定方法失败", new ParameterCountMismatchedException($"参数计数不匹配。应有{paramCount}个，但是现在有{invokeParamCount}个。"));
            }
            return false;
        }
        public static Type[] GetParameterTypes(params ParameterInfo[] parameters)
        {
            List<Type> paramTypes = new List<Type>();
            foreach (var parameterInfo in parameters)
            {
                paramTypes.Add(parameterInfo.ParameterType);
            }
            return paramTypes.ToArray();
        }

        public static Type[] GetArgumentTypes(object[] args)
        {
            List<Type> paramTypes = new List<Type>();
            foreach (var parameterInfo in args)
            {
                paramTypes.Add(parameterInfo.GetType());
            }
            return paramTypes.ToArray();
        }
    }
}
