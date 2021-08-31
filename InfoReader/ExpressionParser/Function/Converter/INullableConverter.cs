namespace InfoReaderPlugin.ExpressionParser.Function.Converter
{
    public interface INullableConverter<T>:IConverterBase where T:struct
    {
        new T? StoredValue { get; }
        new T? Convert(string str);
    }
}