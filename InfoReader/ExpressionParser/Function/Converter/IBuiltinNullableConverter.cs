namespace InfoReaderPlugin.ExpressionParser.Function.Converter
{
    interface IBuiltinNullableConverter<T> : IConverterBase where T: struct
    {
        new T? Value { get; }
        new T? Convert(string str);
    }
}