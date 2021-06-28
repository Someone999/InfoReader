using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osuTools.Exceptions;

namespace InfoReaderPlugin.Exceptions
{
    public class ParameterTypesMismatchedException:osuToolsExceptionBase
    {
        public ParameterTypesMismatchedException(string msg) : base(msg)
        {
        }
        public ParameterTypesMismatchedException(string msg,Exception innerException) : base(msg,innerException)
        {
        }
    }
}
