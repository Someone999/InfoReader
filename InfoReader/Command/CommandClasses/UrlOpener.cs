using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InfoReaderPlugin.I18n;
using osuTools.GameInfo;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    class UrlOpener:ICommandProcessor
    {
        public bool AutoCatch { get; set; } = true;
        public string MainCommand => "open";
        void Open(string url)
        {
            ProcessStartInfo startupInfo = new ProcessStartInfo { FileName = url };
            System.Diagnostics.Process.Start(startupInfo);
        }
        public bool Process(InfoReader instance, CommandParser parser)
        {
            var or = instance.GetOrtdp();
            if (parser.MainCommand == "open")
            {
                if (parser.Arguments[0] == "beatmap")
                {
                    if (parser.Arguments[1] == "page")
                        Open(instance.GetOrtdp().Beatmap.DownloadLink);
                    
                }
                else if (parser.Arguments[1] == "beatmapfolder")
                    Open(or.Beatmap.BeatmapFolder);

                else if (parser.Arguments[0] == "musicfolder")
                    Open(instance.Setting.DefaultMusicCopyingDirectory);

                else if (parser.Arguments[0] == "videofolder")
                    Open(instance.Setting.DefaultVideoCopyingDirectory);

                else if (parser.Arguments[0] == "bgfolder")
                    Open(instance.Setting.DefaultBackgroundCopyingDirectory);
                else if (parser.Arguments[0] == "oszfolder")
                    Open(instance.Setting.OszDir);

                else if (parser.Arguments[0] == "userpage")
                {
                    string username;
                    if(parser.Arguments.Count < 1)
                    {
                        OsuInfo info = new OsuInfo();
                        username = info.UserName;
                        if (string.IsNullOrEmpty(username))
                        {
                            IO.CurrentIO.WriteColor("无法检测用户名，请在最后填入用户名",ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        username = CombineString(parser.Arguments.Skip(1).ToArray());
                    }
                    Open($"https://osu.ppy.sh/users/{username}");
                }
                else
                {
                    IO.CurrentIO.Write(GetHelp());
                }
            }
            return true;
        }
        string CombineString(string[] toCombine, char delim = ' ')
        {
            StringBuilder builder = new StringBuilder();
            if (toCombine.Length == 1)
                return toCombine[0];
            for (int i = 0; i < toCombine.Length; i++)
            {
                if (i <= toCombine.Length - 1 && i != 0)
                    builder.Append(delim);
                builder.Append(toCombine[i]);
            }
            return builder.ToString();
        }
        public string GetHelp() =>
            NI18n.GetLanguageElement("LANG_HELP_OPENER") +
            "getinfo open beatmap <page> [BeatmapId]\n" +
            "getinfo open userpage [username]\n" +
            "getinfo open <beatmapfolder|musicfolder|bgfolder|videofolder|oszfolder>";
    }
}
