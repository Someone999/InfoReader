using System.Text;
using InfoReaderPlugin.I18n;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    class Help:ICommandProcessor
    {
        public string MainCommand => "help";
        public bool AutoCatch { get; set; } = true;
        private InfoReader _infoReader;
        public bool Process(InfoReader instance, CommandParser parser)
        {
            _infoReader = instance;
            if (parser.Arguments.Count <= 0)
            {
                IO.CurrentIO.Write(GetHelp());
                return true;
            }

            var commands = instance.CommandProcessors[parser.Arguments[0]];
            if(!(commands is null))
                IO.CurrentIO.Write(commands.GetHelp());
            return true;
        }

        public string GetHelp()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(NI18n.GetLanguageElement("LANG_HELP_GETINFO_EXTRA"));
            builder.AppendLine(NI18n.GetLanguageElement("LANG_INFO_AVAILABLECMDS"));
            foreach (var command in _infoReader.CommandProcessors.Processors)
                builder.AppendLine(command.Value.MainCommand);
            return builder.ToString();
        }
    }
}
