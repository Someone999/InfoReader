using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using InfoReaderPlugin.StatusMmfPair;
using Sync.Tools;

namespace InfoReaderPlugin
{
    partial class InfoReader
    {
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
                        lastMmf?.OnMmfChanged(lastMmf,currentMmf);
                        _fileFormat = currentMmf.FormatString;
                        CurrentStatusMmf = currentMmf;
                        _lastMmfStatus = _ortdpWrapper.CurrentStatus;
                    }

                    _ortdpWrapper.DebugMode = Setting.DebugMode.ToBool();
                    _ortdpWrapper.BeatmapReadMethod =
                        (osuTools.OrtdpWrapper.OrtdpWrapper.BeatmapReadMethods) Enum.Parse(
                            typeof(osuTools.OrtdpWrapper.OrtdpWrapper.BeatmapReadMethods), Setting.BeatmapReadMethod);
                    if (string.IsNullOrEmpty(Setting.GameDir))
                    {
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
                }
                catch (NullReferenceException e)
                {
                    IO.CurrentIO.WriteColor(e.ToString(),ConsoleColor.Red);
                }
            }
        }
        
    }
}