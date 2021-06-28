using System;
using InfoReaderPlugin.Command.CommandClasses;
using InfoReaderPlugin.I18n;
using osuTools.Exceptions;

namespace InfoReaderPlugin.Exceptions
{
    public class CommandProcessorException:osuToolsExceptionBase
    {
        public CommandProcessorException(string msg) : base(msg)
        {
        }

        public override string Message { get; }


        public CommandProcessorException(string msg,ICommandProcessor processor, Exception innerException) : base(msg, innerException)
        {
            if (msg == NI18n.GetLanguageElement("LANG_ERR_PROCESSPREXCEPTION"))
                Message = string.Format(msg, processor.MainCommand, innerException.Message);
        }
    }
}
