using System;
using InfoReaderPlugin.I18n;

namespace InfoReaderPlugin.Command.CommandClasses
{
    public class VarSearcher:ICommandProcessor
    {
        public string MainCommand => "search";
        public bool AutoCatch { get; set; } = true;
        public bool OnUnhandledException(InfoReader instance, Exception exception)
        {
            return false;
        }
        public string GetHelp() => NI18n.GetLanguageElement("LANG_HELP_SEARCH");

        void Search(InfoReader infoReader, string varName)
        {
            var p = infoReader.GetOrtdp().GetType().GetProperties();
            Sync.Tools.IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_VARSEARCHRESULT"));
            foreach (System.Reflection.PropertyInfo info in p)
            {
                string argupper = varName.ToUpper();
                string perupper = info.Name.ToUpper();
                if (perupper.Contains(argupper))
                {
                    Sync.Tools.IO.CurrentIO.Write(info.Name, false, false);
                    Sync.Tools.IO.CurrentIO.Write(" ", false, false);
                }
            }
            Sync.Tools.IO.CurrentIO.Write("\n", true, false);
        }
        public bool Process(InfoReader infoReader, CommandParser parser)
        {
            Search(infoReader, parser.Arguments[0]);
            return true;
        }
    }
}