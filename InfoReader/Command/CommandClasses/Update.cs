using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using InfoReaderPlugin.Command.Tools;
using InfoReaderPlugin.I18n;
using osuTools.Attributes;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    public class Update:ICommandProcessor
    {
        public static void CheckUpdate(bool forced = false)
        {
            try
            {
                IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_CHECKUPDATE"));
                if (!UpdateHelper.IsLatestVersion() || forced)
                {
                    UpdateHelper.DownloadFiles(PluginVersion.LatestVersion);
                    IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_UPDATED"));
                    IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_NEEDTORESTART"));
                }
            }
            catch (Exception)
            {
                // ignored
            }
            
        }

        internal static string PluginServerUrl = "http://archaring.xyz/plugins";
        public bool OnUnhandledException(InfoReader instance, Exception exception)
        {
            IO.CurrentIO.WriteColor(exception.Message,ConsoleColor.Red);
            return false;
        }

        public bool AutoCatch { get; set; } = true;
        public string MainCommand => "update";
        public bool Process(InfoReader instance, CommandParser parser)
        {
            
            var args = parser.Arguments;
            if (args.Count < 1)
            {
                CheckUpdate(true);
                return true;
            }

            if (args[0] == "changelog")
            {
                if (args.Count == 1)
                {
                    try
                    {
                        PluginVersion p = new PluginVersion(args[1]);
                        IO.CurrentIO.Write(p.GetChangelog());
                        return true;
                    }
                    catch (FormatException)
                    {
                        IO.CurrentIO.WriteColor("Wrong format of version. Must be a.b.c", ConsoleColor.Red);
                    }
                }
            }

            if (args[0] == "download")
            {
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
                            IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_WARN_DOWNGRADE"),ConsoleColor.Yellow);
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
                        IO.CurrentIO.WriteColor("Wrong format of version. Must be a.b.c", ConsoleColor.Red);
                    }
                }
            }

            return true;
        }

        public string GetHelp()
        {
           return NI18n.GetLanguageElement("LANG_HELP_UPDATE");
        }
    }
}