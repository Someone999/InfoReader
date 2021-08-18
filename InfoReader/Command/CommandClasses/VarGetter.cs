using System;
using System.Threading;
using System.Threading.Tasks;
using InfoReaderPlugin.I18n;
using InfoReaderPlugin.Window;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    public static class VarTools
    {
        public static object GetVariableValue(InfoReader infoReader, string varName, ref bool exist)
        {
            Type t = infoReader.GetOrtdp().GetType();
            var properties = t.GetProperties();
            foreach (var p in properties)
            {
                if (p.Name == varName)
                {
                    exist = true;
                    return p.GetValue(infoReader.GetOrtdp());
                }
            }
            return null;
        }
    }
    public class VarGetter:ICommandProcessor
    {
        public string MainCommand => "var";
        public bool AutoCatch { get; set; } = true;
        public bool OnUnhandledException(InfoReader instance, Exception exception)
        {
            return false;
        }
        public string GetHelp() => NI18n.GetLanguageElement("LANG_HELP_VARGETTER") + "\n" + NI18n.GetLanguageElement("LANG_HELP_VARGUI");
        private VariableViewer _viewer;
        private bool _msgLoopStarted;
        public bool Process(InfoReader instance,CommandParser parser)
        {
            if (_viewer is null)
                _viewer = new VariableViewer(instance.GetOrtdp());

            if (parser.MainCommand == "var")
            {
                if(parser.Arguments[0] == "gui")
                {
                    if(!_msgLoopStarted)
                    {
                        if (instance.Setting.DebugMode.ToString().Equals("True",StringComparison.OrdinalIgnoreCase))
                            IO.CurrentIO.Write($"Message loop on window VariableViewer started.");

                        Thread th = new Thread(() =>
                        {
                            System.Windows.Forms.Application.Run(_viewer);
                        });
                        th.Start();
                        if (th.ThreadState == ThreadState.Running)
                            _msgLoopStarted = true;
                        return true;
                    }

                    if (instance.Setting.DebugMode.ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
                        IO.CurrentIO.Write($"Message loop on VariableViewer is running. Showing window...");
                    _viewer.Show();
                    return true;
                }
                int match = 0;
                var s = parser.Arguments[0];
                Type t = instance.GetOrtdp().GetType();
                var ps = t.GetProperties();
                string[] gets = s.Split('.');
                object currentObject = instance.GetOrtdp();
                var currentProperties = ps;
                foreach (var name in gets)
                {
                    foreach (var property in currentProperties)
                        if (property.Name == name)
                        {
                            currentObject = property.GetValue(currentObject);
                            currentProperties = currentObject.GetType().GetProperties();
                            match++;
                        }
                }

                IO.CurrentIO.Write(match == gets.Length
                    ? $"{s}:{currentObject}"
                    : NI18n.GetLanguageElement("LANG_ERR_VARNOTFOUND"));
                return true;
            }

            
            return true;
        }
    }
}