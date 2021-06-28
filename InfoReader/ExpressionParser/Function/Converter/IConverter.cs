﻿using System.Collections.Generic;

namespace InfoReaderPlugin.ExpressionParser.Function.Converter
{
    public interface IConverter<T> : IConverterBase
    {
        new T Value { get; }
        new T Convert(string str);
    }
}