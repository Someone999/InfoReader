using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osuTools.Exceptions;

namespace InfoReaderPlugin.Exceptions
{
    public class InvokeFailedException:osuToolsExceptionBase
    {
        public InvokeFailedException(string msg):base(msg)
        {
        }
        public InvokeFailedException(string msg,Exception innerException) : base(msg,innerException)
        {
        }
    }
}
