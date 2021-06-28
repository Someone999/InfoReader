using System;
using System.Collections.Generic;
using System.Reflection;

namespace InfoReaderPlugin.ExpressionParser
{
    public class TypeConstructorInfo
    {
        public TypeConstructorInfo(object existedValue = null)
        {
            ExistedValue = existedValue;
            if (existedValue is object obj)
            {
                TargeType = obj.GetType();
            }
        }
        public TypeConstructorInfo(Type type,bool isStatic,List<Type> argumentTypes = null,List<object> arguments = null)
        {
            TargeType = type ?? throw new ArgumentNullException(nameof(type),@"type不能为null");
            ArgumentTypes = argumentTypes ?? new List<Type>();
            Arguments = arguments ?? new List<object>();
            IsStatic = isStatic;
        }
        public Type TargeType { get; set; }
        public List<Type> ArgumentTypes { get; set; } = new List<Type>();
        public List<object> Arguments { get; set; } = new List<object>();
        public bool IsStatic { get; set; }
        public object ExistedValue { get; private set; }
        public object CreateInstance(bool replaceExistedValue = false)
        {
            
            if (IsStatic || TargeType.IsAbstract)
                return default;
            ConstructorInfo constructor = TargeType.GetConstructor(ArgumentTypes.ToArray());
            if (constructor is null)
                throw new MissingMethodException("找不到指定的构造函数");
            object newIns = constructor.Invoke(Arguments.ToArray());
            if (replaceExistedValue)
                ExistedValue = newIns;
            return newIns;
        }

        public T CreateInstance<T>(bool replaceExistedValue = false) => (T) CreateInstance(replaceExistedValue);
    }
}