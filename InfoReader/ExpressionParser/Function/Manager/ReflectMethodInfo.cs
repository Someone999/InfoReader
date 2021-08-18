using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace InfoReaderPlugin.ExpressionParser.Function.Manager
{
    public class ReflectMethodInfo
    {
        public ReflectMethodInfo(MethodInfo method,object reflectObject = null)
        {
            ReflectObject = reflectObject;
            Method = method ?? throw new ArgumentNullException(nameof(method), @"反射方法不能为null");
        }
        public object ReflectObject { get; set; }
        public MethodInfo Method { get; set; }

        public object Invoke(out bool success,params object[] parameters)
        {
            List<Type> paramsTypes = new List<Type>(from p in parameters select p.GetType());
            var paramList = Method.GetParameters();
            var realParamTypes = (from param in paramList select param.ParameterType).ToArray();
            bool countMatched = MethodTools.CheckParameterCount(Method.Name,parameters.Length,paramList.Length,true);
            bool typesMatched = MethodTools.CheckParameterTypes(Method.Name, paramsTypes.ToArray(),realParamTypes,true);
            if (!(countMatched && typesMatched))
            {
                success = false;
                return null;
            }

            success = true;
            return Method.Invoke(ReflectObject, parameters);
        }
    }
}