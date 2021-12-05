using System;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using InfoReaderPlugin.I18n;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    class Config:ICommandProcessor
    {
        private readonly MemFileMapFormat _formatWindow = new MemFileMapFormat();
        private bool _msgLoopStarted;
        public bool OnUnhandledException(InfoReader instance, Exception exception)
        {
            return false;
        }

        public string MainCommand => "config";
        public bool AutoCatch { get; set; } = true;
        public string GetHelp() => NI18n.GetLanguageElement("LANG_HELP_MMFCONFIG");
        public bool Process(InfoReader instance, CommandParser parser)
        {
            if(!_msgLoopStarted)
            {
                if (instance.Setting.DebugMode.ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
                    IO.CurrentIO.Write("Message loop on window MemFileMapFormat started.");
                Thread th = new Thread(() =>
                {
                    _formatWindow.OnSaved += () => UpdateMmfList(instance, parser);
                    System.Windows.Forms.Application.Run(_formatWindow);
                });
                th.Start();
                if (th.ThreadState == ThreadState.Running)
                    _msgLoopStarted = true;
                return true;
            }
            if (instance.Setting.DebugMode.ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
                IO.CurrentIO.Write("Message loop on window MemFileMapFormat is running. Showing window...");
            _formatWindow.Show();
            return true;
        }

        internal static void UpdateMmfList(InfoReader instance, CommandParser parser)
        {
            bool debugMode = instance.Setting.DebugMode.ToBool();
            if (debugMode)
                IO.CurrentIO.Write("[InfoReader] Mmf format has been refreshed.");
            StringBuilder info = new StringBuilder();
            foreach (var instanceExpressionMatcher in instance.ExpressionMatchers)
            {
                var matcher = instanceExpressionMatcher.Value;
                matcher.Match(instance.CurrentStatusMmf.FormatString);

                if (debugMode)
                {
                    info.Append($"Current Matcher: {matcher.GetType().Name}\nMatched count: {matcher.Results.Length}\n" +
                                       $"Current Status: {instance.CurrentStatusMmf.TargetStatus}\n");
                    IO.CurrentIO.Write(info.ToString());
                }
            }
            

        }
    }
}
