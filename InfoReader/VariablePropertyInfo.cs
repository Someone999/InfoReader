using System.Collections.Generic;
using System.Reflection;

namespace InfoReaderPlugin
{
    public class VariablePropertyInfo:IEqualityComparer<VariablePropertyInfo>
    {
        public PropertyInfo Property { get; }
        public string Alias { get; }
        //public object ReflectObject { get; set; }
        public string VariableName { get; set; }
        public override int GetHashCode()
        {
            int part1 = Property is null ? 0 : Property.GetHashCode();
            int part2 = string.IsNullOrEmpty(Alias) ? 0 : Alias.GetHashCode();
            long hashCode = part1 + part2;
            if (hashCode > int.MaxValue)
                hashCode -= int.MaxValue;
            else if (hashCode < int.MinValue)
                hashCode += int.MaxValue;
            return (int) hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is VariablePropertyInfo v)
                return v.Alias == Alias && v.Property == Property;
            return false;
        }

        public bool Equals(VariablePropertyInfo a,VariablePropertyInfo b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.Equals(b);
        }

        public int GetHashCode(VariablePropertyInfo variablePropertyInfo) => variablePropertyInfo.GetHashCode();
        public VariablePropertyInfo(string alias,string variableName,PropertyInfo property/*,object reflectObject*/)
        {
            Alias = alias;
            Property = property;
            VariableName = variableName;
        }

    }
}
