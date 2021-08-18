using InfoReaderPlugin.ExpressionParser.Function.Converter;

namespace InfoReaderPlugin.Config.Converters
{
    public class BoolConfigConverter :  IConverter<bool>
    {
        public string ConverterName => "Internal_Config_Bool_Converter";
        public bool Value { get; private set; }

        public BoolConfigConverter(bool initValue)
        {
            Value = initValue;
        }
        object IConverterBase.Value => Value;

        object IConverterBase.Convert(string str)
        {
            return Value = Convert(str);
        }

        public bool Convert(string str) => bool.Parse(str);

        public string ConvertToString() => Value.ToString();
    }
}