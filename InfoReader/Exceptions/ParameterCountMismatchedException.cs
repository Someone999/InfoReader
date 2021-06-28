using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osuTools.Exceptions;

namespace InfoReaderPlugin.Exceptions
{
    public class ParameterCountMismatchedException:osuToolsExceptionBase
    {
        public ParameterCountMismatchedException(string msg) : base(msg)
        {
        }
        public ParameterCountMismatchedException(string msg,Exception innerException) : base(msg,innerException)
        {
        }
    }
}
