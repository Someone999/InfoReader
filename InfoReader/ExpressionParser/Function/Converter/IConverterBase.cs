namespace InfoReaderPlugin.ExpressionParser.Function.Converter
{

    public interface IConverterBase
    {
        string ConverterName { get;  }
        object Value { get; }
        object Convert(string str);
        string ConvertToString();
    }
}