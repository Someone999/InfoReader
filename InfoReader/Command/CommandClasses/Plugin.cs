using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReaderPlugin.Command.Tools;
using InfoReaderPlugin.I18n;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    public class Plugin:ICommandProcessor
    {
        public bool OnUnhandledException(InfoReader instance, Exception exception)
        {
            return false;
        }

        bool Downgrade(InfoReader instance, CommandParser parser)
        {
            var args = parser.Arguments;
            if (args.Count < 1)
                    IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_ERR_NOVERSION"), ConsoleColor.Yellow);
            if (args.Count == 2)
            {
                try
                {
                    PluginVersion p = new PluginVersion(args[1]);
                    if (p == PluginVersion.CurrentVersion)
                    {
                        IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_VERSIONISSAME"));
                        return true;
                    }

                    if (p < new PluginVersion("1.0.18"))
                    {
                        IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_WARN_DOWNGRADE"), ConsoleColor.Yellow);
                        IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_WARN_CONFIRMDOWNGRADE"), ConsoleColor.Yellow);
                        string s = IO.CurrentIO.ReadCommand();
                        if (s == "yes")
                        {
                            instance.Setting.AutoUpdate = "False";
                            UpdateHelper.DownloadFiles(p);
                        }
                        IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_NEEDTORESTART"));
                        return true;
                    }

                }
                catch (FormatException)
                {
                    IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_ERR_VERSIONFORMAT"), ConsoleColor.Red);
                }
            }

            return true;
        }


        public bool AutoCatch { get; set; } = true;
        public string MainCommand => "plugin";
        public bool Process(InfoReader instance, CommandParser parser)
        {
            var args = parser.Arguments;
            if (args.Count == 1)
            {
                if (args[0] == "version")
                    IO.CurrentIO.Write(string.Format(NI18n.GetLanguageElement("LANG_INFO_CURRENTVERSION"),PluginVersion.CurrentVersion));
            }

            if (args.Count == 2)
            {
                if (args[0] == "download")
                    return Downgrade(instance, parser);
            }

            return true;
        }

        public string GetHelp()
        {
            return NI18n.GetLanguageElement("LANG_HELP_PLUGIN");
        }
    }
}
