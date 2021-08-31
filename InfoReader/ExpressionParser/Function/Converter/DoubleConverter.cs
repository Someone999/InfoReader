using System.Globalization;

namespace InfoReaderPlugin.ExpressionParser.Function.Converter
{
    class DoubleConverter:IBuiltinConverter<double>
    {
        public string ConverterName => "BuiltinDoubleConverter";

        object IConverterBase.StoredValue => StoredValue;

        public double StoredValue { get; private set; }
        object IConverterBase.Convert(string str) => Convert(str);

        public double Convert(string str)
        {
            if (double.TryParse(str, out var rslt))
                StoredValue = rslt;
            return rslt;
        }

        public string ConvertToString() => StoredValue.ToString(CultureInfo.CurrentCulture);
    }
}
