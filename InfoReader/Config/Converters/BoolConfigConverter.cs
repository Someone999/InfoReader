using InfoReaderPlugin.ExpressionParser.Function.Converter;

namespace InfoReaderPlugin.Config.Converters
{
    public class BoolConfigConverter :  IConverter<bool>
    {
        public string ConverterName => "Internal_Config_Bool_Converter";
        public bool StoredValue { get; private set; }

        public BoolConfigConverter(bool initValue)
        {
            StoredValue = initValue;
        }
        object IConverterBase.StoredValue => StoredValue;

        object IConverterBase.Convert(string str)
        {
            return StoredValue = Convert(str);
        }

        public bool Convert(string str) => StoredValue = bool.Parse(str);

        public string ConvertToString() => StoredValue.ToString();
    }
}