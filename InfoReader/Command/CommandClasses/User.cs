using System;
using System.Linq;
using System.Text;
using InfoReaderPlugin.I18n;
using osuTools.Game.Modes;
using osuTools.OnlineInfo.OsuApiV1.OnlineQueries;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    class User:ICommandProcessor
    {
        public string MainCommand => "user";
        public bool AutoCatch { get; set; } = true;
        bool IsModeString(string s,out int mode)
        {
            mode = -1;
            bool osuMode = s.Equals("osu", StringComparison.OrdinalIgnoreCase);
            bool taikoMode = s.Equals("taiko", StringComparison.OrdinalIgnoreCase);
            bool catchMode = s.Equals("catch", StringComparison.OrdinalIgnoreCase) ||s.Equals("ctb", StringComparison.OrdinalIgnoreCase);
            bool maniaMode = s.Equals("mania", StringComparison.OrdinalIgnoreCase);
            if (osuMode) mode = 0;
            if (taikoMode) mode = 1;
            if (catchMode) mode = 2;
            if (maniaMode) mode = 3;
            return osuMode || taikoMode || catchMode || maniaMode;
        }

        string CombineString(string[] toCombine,char delim = ' ')
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
        public bool Process(InfoReader instance, CommandParser parser)
        {
            string apiKey = instance.Setting.ApiKey;
            if(parser.MainCommand == "user")
            {
                if (parser.Arguments[0] == "show")
                {
                    if (IsModeString(parser.Arguments.Last(),out int mode))
                    {
                        OnlineUserQuery query = new OnlineUserQuery
                        {
                            Mode = (OsuGameMode) mode,
                            UserName = CombineString(parser.Arguments.GetRange(1,parser.Arguments.Count - 2).ToArray()),
                            OsuApiKey = apiKey
                        };
                        try
                        {
                            if (query.UserInfo.UserId == 0)
                            {
                                string format = NI18n.GetLanguageElement("LANG_ERR_FAILTOQUERYUSERINFO");
                                throw new osuTools.Exceptions.OnlineQueryFailedException(string.Format(format,
                                    string.IsNullOrEmpty(query.UserName) ? query.UserId.ToString() : query.UserName));
                            }
                            IO.CurrentIO.Write(query.UserInfo.ToString());
                        }
                        catch (Exception e)
                        {
                            IO.CurrentIO.WriteColor(e.Message,ConsoleColor.Red);
                        }
                        
                        
                    }
                    else
                    {
                        IO.CurrentIO.WriteColor("模式必须在最后一个参数的位置被指定。",
                            ConsoleColor.Red);
                        return true;
                    }
                }
            }

            return true;
        }

        public string GetHelp() => NI18n.GetLanguageElement("LANG_HELP_GETUSERINFO");
    }
}
