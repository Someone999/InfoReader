using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using InfoReaderPlugin.Command.Tools;
using InfoReaderPlugin.I18n;
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

        Process UpdateProcess(InfoReader instance,CommandParser parser)
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
            return osuprocesses[0];
        }
        bool ContainsModule(string moduleName,Process process)
        {
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName.ToUpper().Contains(moduleName.ToUpper()))
                {
                    return true;
                }
            }

            return false;
        }

        void CheckFile()
        {
            if (!File.Exists("injector.exe"))
                File.WriteAllBytes("injector.exe", Resource1.FallbackInjector);
        }
        public bool Reinject(InfoReader instance,CommandParser parser)
        {
            try
            {
                
                var osuprocess = UpdateProcess(instance, parser);
                var osu = new HandleTypes.ProcessHandle(osuprocess);
                if (ContainsModule("Overlay.dll",osuprocess))
                {
                    IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_HASINJECTED"));
                    return true;
                }
                Injector injector = new Injector(osu.PID, $"{Environment.CurrentDirectory}\\..\\overlay.dll");
                var res = injector.Inject();
                Stopwatch s = new Stopwatch();
                if (res)
                {
                    s.Start();
                    while (s.Elapsed.TotalSeconds < 10)
                    {
                        osuprocess = UpdateProcess(instance, parser);
                        bool c = ContainsModule("Overlay.dll", osuprocess);
                        if (!c) 
                            continue;
                        if (instance.Setting.DebugMode.ToString()
                            .Equals("True", StringComparison.OrdinalIgnoreCase))
                        {
                            IO.CurrentIO.WriteColor($"Injected in {s.ElapsedMilliseconds} ms", ConsoleColor.DarkGreen);
                            return true;
                        }

                        return true;
                    }
                    if (instance.Setting.DebugMode.ToString()
                        .Equals("True", StringComparison.OrdinalIgnoreCase))
                        IO.CurrentIO.WriteColor("The first try timed out. Starting next ...", ConsoleColor.Red);
                    
                    var cinjector = System.Diagnostics.Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = "injector",
                            Arguments = $"\"{Environment.CurrentDirectory}\\..\\overlay.dll\" {osuprocess.Id}",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                    
                    CheckFile();
                    s.Restart();
                    while (s.Elapsed.TotalSeconds < 0.5)
                    {
                        osuprocess = UpdateProcess(instance, parser);
                        bool c = ContainsModule("Overlay.dll", osuprocess);
                        if (instance.Setting.DebugMode.ToString()
                            .Equals("True", StringComparison.OrdinalIgnoreCase) && c)
                        {
                            IO.CurrentIO.WriteColor($"Injected in {s.ElapsedMilliseconds} ms", ConsoleColor.DarkGreen);
                        }
                    }
                    //之前这个return漏掉了
                    return true;
                }
                string msg = NI18n.GetLanguageElement("LANG_INJECTIONFAILED");
                msg += $"({WinAPI.ErrorHandle.GetLastError()}),{WinAPI.ErrorHandle.GetErrorString(WinAPI.ErrorHandle.FormatMessageFlags.FromSystem)}";
                IO.CurrentIO.WriteColor(msg, ConsoleColor.Red);
                return true;
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
        public bool Process(InfoReader instance, CommandParser parser)
        {
            var plugins = Sync.SyncHost.Instance.EnumPluings();
            if (plugins.Any(p => p.Name == "IngameOverlay")) 
                return Reinject(instance, parser);
            IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_ERR_NOINGAMEOVERLAY"),ConsoleColor.Red);
            return true;
        }

        public string GetHelp() => NI18n.GetLanguageElement("LANG_HELP_REINJECT");
    }
}