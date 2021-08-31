namespace InfoReaderPlugin.ExpressionParser.Function.Converter
{
    interface IBuiltinNullableConverter<T> : IConverterBase where T: struct
    {
        new T? StoredValue { get; }
        new T? Convert(string str);
    }
}