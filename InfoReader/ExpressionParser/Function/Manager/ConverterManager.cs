using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using InfoReaderPlugin.ExpressionParser.Function.Converter;

namespace InfoReaderPlugin.ExpressionParser.Function.Manager
{
    public class ConverterManager
    {
        internal static ConverterManager GetInstance() => _converterManager ?? (_converterManager = new ConverterManager());

        private readonly Dictionary<string, IConverterBase> _converters = new Dictionary<string, IConverterBase>();
        private static ConverterManager _converterManager;

        private ConverterManager()
        {
        }
        public IConverterBase GetConverter(string converterName)
        {
            if (_converters.ContainsKey(converterName))
                return _converters[converterName];
            return null;
        }

        public T GetConverter<T>(string converterName) => (T) GetConverter(converterName);
        public bool Register(IConverterBase converter,bool removeBuiltin)
        {
            bool removed = false;
            if (_converters.ContainsKey(converter.ConverterName))
            {
                if (_converters[converter.ConverterName] is IBuiltinConverterBase builtinConverter)
                    if (removeBuiltin)
                        removed = _converters.Remove(builtinConverter.ConverterName);
                if (removed)
                {
                    _converters.Add(converter.ConverterName, converter);
                    return true;
                }
            }
            else return false;
            _converters.Add(converter.ConverterName, converter);
            return true;
        }
    }
}
