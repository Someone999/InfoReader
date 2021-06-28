using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReaderPlugin.ExpressionParser.Function.Converter
{
    interface IBuiltinConverter<T>:IBuiltinConverterBase
    {
        new T Value { get; }
        new T Convert(string str);
    }
}
