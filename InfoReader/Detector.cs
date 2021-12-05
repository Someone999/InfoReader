using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using InfoReaderPlugin.Command.CommandClasses;
using InfoReaderPlugin.StatusMmfPair;
using osuTools.GameInfo;
using Sync.Tools;

namespace InfoReaderPlugin
{
    partial class InfoReader
    {
        [DllImport("kernel32")]
        static extern bool GetExitCodeProcess(IntPtr hProcess, out int exitCode);
        private void GameExitCodeDetector()
        {
            OsuInfo info = null;
            Process osuProcess = null;
            while (info?.CurrentProcess is null)
            {
                Thread.Sleep(3000);
                info = new OsuInfo();
                osuProcess = info.CurrentProcess;
            }
            IO.CurrentIO.WriteColor("Found osu! process", ConsoleColor.Yellow);
            osuProcess.WaitForExit();
            int? mayBeCode = null;
            int code;
            if (GetExitCodeProcess(osuProcess.Handle, out code))
            {
                mayBeCode = code;
                if (code != 0)
                    IO.CurrentIO.WriteColor($"Process exited abnormally. Exit code: 0x{code:x8}", ConsoleColor.Red);
            }
            if (mayBeCode == null)
                IO.CurrentIO.WriteColor($"Can not get exit code of osu!", ConsoleColor.Red);
        }

        private void ConigFileWatcher()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    if (_lastMmfStatus != _ortdpWrapper.CurrentStatus)
                    {
                        var lastMmf = CurrentStatusMmf;
                        var currentMmf = StatusMmf.GetMmfByStatus(_ortdpWrapper.CurrentStatus);
                        lastMmf?.OnMmfChanged(lastMmf, currentMmf);
                        CurrentStatusMmf = currentMmf;
                        Command.CommandClasses.Config.UpdateMmfList(this, null);
                        _lastMmfStatus = _ortdpWrapper.CurrentStatus;
                    }
                    _fileFormat = CurrentStatusMmf?.FormatString ?? "";
                    _ortdpWrapper.DebugMode = Setting.DebugMode.ToBool();
                    _ortdpWrapper.BeatmapReadMethod =
                        (osuTools.OrtdpWrapper.OrtdpWrapper.BeatmapReadMethods) Enum.Parse(
                            typeof(osuTools.OrtdpWrapper.OrtdpWrapper.BeatmapReadMethods), Setting.BeatmapReadMethod);
                    if (!string.IsNullOrEmpty(Setting.GameDir)) 
                        continue;
                    Process[] processes = Process.GetProcessesByName("osu!");
                    if (processes.Length != 0)
                    {
                        var path = Path.GetDirectoryName(processes[0].MainModule?.FileName);
                        if (string.IsNullOrEmpty(path))
                            IO.CurrentIO.WriteColor("Can not get osu! path! Please insert it manually.",
                                ConsoleColor.Yellow);
                        else
                        {
                            if (File.Exists(Path.Combine(path, $"osu!.{Environment.UserName}.cfg")))
                                Setting.GameDir = processes[0].MainModule.FileName;
                            else
                            {
                                IO.CurrentIO.WriteColor("Can not get osu! path! Please insert it manually.",
                                    ConsoleColor.Yellow);
                            }
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                    IO.CurrentIO.WriteColor(e.ToString(),ConsoleColor.Red);
                }
            }
        }
        
    }
}