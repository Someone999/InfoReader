using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using InfoReaderPlugin.Exceptions;
using InfoReaderPlugin.ExpressionParser.Function.Attributes;

namespace InfoReaderPlugin.ExpressionParser.Function.Manager
{
    public class FunctionManager
    {
        private static FunctionManager _functionManager;
        public static FunctionManager GetInstance() => _functionManager ?? (_functionManager = new FunctionManager());
        private readonly List<ReflectMethodInfo> _methods = new List<ReflectMethodInfo>();

        public bool RegisterType(TypeConstructorInfo constructorInfo,bool rethrow)
        {
            if (!constructorInfo.TargeType.IsClass)
                return false;
            if (constructorInfo.TargeType.IsAbstract)
                return false;
            if (constructorInfo.ExistedValue is null)
                constructorInfo.CreateInstance(true);
            MethodInfo[] methods = constructorInfo.TargeType.GetMethods(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Public);
            foreach (var methodInfo in methods)
            {
                try
                {
                    var attributes = methodInfo.GetCustomAttributes();
                    var funcAttr = (InfoReaderFunctionAttribute)attributes.FirstOrDefault(attr => attr is InfoReaderFunctionAttribute);
                    if (funcAttr is null)
                        continue;
                    ReflectMethodInfo reflectMethodInfo = new ReflectMethodInfo
                    (
                        methodInfo,
                        constructorInfo.ExistedValue
                    );
                    _methods.Add(reflectMethodInfo);
                }
                catch (Exception)
                {
                    if (rethrow)
                        throw;
                }
            }
            if (_methods.Count == 0)
                return false;
            return true;
        }

        
        ReflectMethodInfo GetReflectMethodInfo(string methodName,Type[] parametersTypes)
        {
            var methods = _methods.Where(m => m.Method.Name == methodName);
            foreach (var method in methods)
            {
                var methodParamsTypes = MethodTools.GetParameterTypes(method.Method.GetParameters());
                var countMatched = MethodTools.CheckParameterCount(methodName,parametersTypes.Length, methodParamsTypes.Length, true);
                var typesMatched = MethodTools.CheckParameterTypes(methodName,parametersTypes, methodParamsTypes, true);
                if (countMatched && typesMatched)
                    return method;
            }
            return null;
        }

        public MethodInfo GetMethod(string methodName, Type[] parametersTypes) =>
            GetReflectMethodInfo(methodName, parametersTypes)?.Method;

        public object InvokeMethod(string methodName, params object[] parameters)
        {
            ReflectMethodInfo method = GetReflectMethodInfo(methodName, MethodTools.GetArgumentTypes(parameters));
            if (method is null)
                throw new MissingMethodException("找不到指定的方法");
            object rslt = method.Invoke(out var success, parameters);
            if(success)
                return rslt;
            throw new InvokeFailedException($"对函数{method}的调用失败。");
        }

    }
}