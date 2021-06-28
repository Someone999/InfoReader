using System.Globalization;

namespace InfoReaderPlugin.ExpressionParser.Function.Converter
{
    class DoubleConverter:IBuiltinConverter<double>
    {
        public string ConverterName => "BuiltinDoubleConverter";

        object IConverterBase.Value => Value;

        public double Value { get; private set; }
        object IConverterBase.Convert(string str) => Convert(str);

        public double Convert(string str)
        {
            if (double.TryParse(str, out var rslt))
                Value = rslt;
            return rslt;
        }

        public string ConvertToString() => Value.ToString(CultureInfo.CurrentCulture);
    }
}
