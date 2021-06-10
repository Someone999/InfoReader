using System;
using System.Diagnostics;
using System.IO;
using InfoReaderPlugin.I18n;
using InfoReaderPlugin.Plugin.Command.Tools;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    public class Reinjector:ICommandProcessor
    {
        public string MainCommand => "reinject";
        public bool AutoCatch { get; set; } = true;
        public bool OnUnhandledException(InfoReader instance, Exception exception)
        {
            return false;
        }
        public bool Process(InfoReader instance, CommandParser parser)
        {
            try
            {
                var osuprocesses = System.Diagnostics.Process.GetProcessesByName("osu!");
                if (osuprocesses.Length == 0)
                {

                    if (string.IsNullOrEmpty(instance.Setting.GameDir))
                    {
                        IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_WAITINGFOROSU"), ConsoleColor.Red);
                        while (osuprocesses.Length == 0)
                        {
                            System.Threading.Thread.Sleep(3000);
                        }
                    }
                    else
                    {
                        IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_STARTOSU"), ConsoleColor.Red);
                        System.Diagnostics.Process.Start(instance.Setting.GameDir);

                    }
                }
                osuprocesses = System.Diagnostics.Process.GetProcessesByName("osu!");
                var osuprocess = osuprocesses[0];
                var osu = new HandleTypes.ProcessHandle(osuprocess);
                foreach (ProcessModule module in osuprocess.Modules)
                {
                    if (module.ModuleName.ToUpper().Contains("Overlay.dll".ToUpper()))
                    {
                        IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_HASINJECTED"));
                        return true;
                    }
                }
                Injector injector = new Injector(osu.PID, $"{Environment.CurrentDirectory}\\..\\overlay.dll");
                var res = injector.Inject();
                Stopwatch s = new Stopwatch();
                if (res)
                {
                    var modules = osuprocess.Modules;
                    s.Start();
                    while (s.Elapsed.TotalSeconds < 0.5)
                    {
                        foreach (ProcessModule module in modules)
                        {
                            if (module.ModuleName.ToUpper().Contains("Overlay.dll".ToUpper()))
                            {
                                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_INJECTED"), ConsoleColor.DarkGreen);
                                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_RESTARTSYNC"), ConsoleColor.DarkGreen);
                                System.Threading.Thread.Sleep(3000);
                                Sync.SyncHost.Instance.RestartSync();
                                return true;
                            }
                        }
                    }
                    if (!File.Exists("injector.exe"))
                        File.WriteAllBytes("injector.exe", Resource1.FallbackInjector);
                    var cinjector = System.Diagnostics.Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = "injector",
                            Arguments = $"\"{Environment.CurrentDirectory}\\..\\overlay.dll\" {osuprocess.Id}",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                    while (s.Elapsed.TotalSeconds < 0.5)
                    {
                        foreach (ProcessModule module in modules)
                        {
                            if (module.ModuleName.ToUpper().Contains("Overlay.dll".ToUpper()))
                            {
                                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_INJECTED"), ConsoleColor.DarkGreen);
                                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_RESTARTSYNC"), ConsoleColor.DarkGreen);
                                System.Threading.Thread.Sleep(3000);
                                Sync.SyncHost.Instance.RestartSync();
                                return true;
                            }
                        }
                    }
                    IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_INJECTED"), ConsoleColor.DarkGreen);
                    IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_RESTARTSYNC"), ConsoleColor.DarkGreen);
                    System.Threading.Thread.Sleep(3000);
                    Sync.SyncHost.Instance.RestartSync();
                    return true;
                }
                else
                {
                    string msg = NI18n.GetLanguageElement("LANG_INJECTIONFAILED");
                    msg += $"({WinAPI.ErrorHandle.GetLastError()}),{WinAPI.ErrorHandle.GetErrorString(WinAPI.ErrorHandle.FormatMessageFlags.FromSystem)}";
                    IO.CurrentIO.WriteColor(msg, ConsoleColor.Red);
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                if (instance.Setting.GameDir == "")
                    IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_ERR_NOOSUDIR"), ConsoleColor.Red);
                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                if (!File.Exists(instance.Setting.GameDir))
                    IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_ERR_NOOSU"), ConsoleColor.Red);
                return true;
            }
        }

        public string GetHelp() => NI18n.GetLanguageElement("LANG_HELP_REINJECT");
    }
}