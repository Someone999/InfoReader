using System;
using System.Runtime.Remoting.Channels;
using System.Threading;
using InfoReaderPlugin.I18n;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    class Config:ICommandProcessor
    {
        private readonly MemFileMapFormat _formatWindow = new MemFileMapFormat();
        private bool _msgLoopStarted;
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
    }
}
