using System;
using System.Collections.Generic;

namespace InfoReaderPlugin.MemoryMapWriter
{
    public class ConstructorArgumentsInfo
    {
        public List<Type> ArgumentTypes { get; set; } = new List<Type>();
        public List<object> Arguments { get; set; } = new List<object>();
    }
}