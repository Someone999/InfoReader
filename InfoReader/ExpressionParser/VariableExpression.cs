using System;
using System.Linq;
using System.Reflection;

namespace InfoReaderPlugin.ExpressionParser
{
    public sealed class VariableExpression : Expression
    {
        public VariableExpression(string expressionString, object target) : base(expressionString, target)
        {
            if(ExpressionString.StartsWith("${"))
            {
                ExpressionString = ExpressionString.TrimStart("${".ToCharArray());
            }
            if (ExpressionString.EndsWith("}"))
            {
                ExpressionString = ExpressionString.TrimEnd("}".ToCharArray());
            }
            if (ExpressionString.Contains(":"))
            {
                string [] strs = ExpressionString.Split(':');
                if (strs.Length > 2)
                {
                    ExpressionString = strs[0];
                    Format = "";
                    return;
                }
                HasFormat = true;
                Format = strs[1];
                NoFormatExpression = strs[0];
            }
            else
            {
                NoFormatExpression = ExpressionString;
            }
        }

        public bool HasFormat { get; }
        public string NoFormatExpression { get; private set; }
        public string Format { get; private set; }
        public PropertyInfo CurrentProperty { get; private set; }
        public override ExpressionTypes Type => ExpressionTypes.Builtin | ExpressionTypes.Variable;
        public override object GetProcessedValue()
        {
            if (double.TryParse(NoFormatExpression, out double val))
            {
                try
                {
                    if (!string.IsNullOrEmpty(Format))
                        return val.ToString(Format);
                }
                catch (FormatException)
                {
                    return ((int) val).ToString(Format);
                }
               
                return val;
            }
            string[] namePartial = NoFormatExpression.Split('.');            
            object t = Target;
            int match = 0;
            if (InfoReader.VariablePropertyInfos.Any(m => m.Alias == NoFormatExpression))
            {
                var variableProperty = (from c in InfoReader.VariablePropertyInfos where c.Alias == NoFormatExpression select c)
                    .FirstOrDefault()??new VariablePropertyInfo("null","null",null);
                NoFormatExpression = variableProperty.VariableName;
                namePartial = NoFormatExpression.Split('.');
            }

            int index = NoFormatExpression.LastIndexOf(".", StringComparison.Ordinal);
            foreach (var partial in namePartial)
            {
                string lowerName = partial.ToLower();

                foreach (var pro in t.GetType().GetProperties())
                {
                    if (pro.Name.ToLower() == lowerName)
                    {
                        CurrentProperty = pro;
                        t = pro.GetValue(t);
                        match++;
                    }
                }
            }
            if (match != namePartial.Length)
                return "Unknown Variable";
            if (!string.IsNullOrEmpty(Format))
            {
                MethodInfo formatedToString = t.GetType().GetMethod("ToString", new [] {typeof(string)});
                if (formatedToString != null)
                    return formatedToString.Invoke(t, new object[] {Format});
            }
            return t;
        }

    }
}