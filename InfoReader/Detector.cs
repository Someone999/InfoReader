using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Sync.Tools;

namespace InfoReaderPlugin
{
    partial class InfoReader
    {
        private void ConigFileWatcher()
        {
            while (true)
            {
                Thread.Sleep(5);
                _fileFormat = _ortdpWrapper.GameStatus.CurrentStatus != osuTools.Game.OsuGameStatus.Unkonwn ? 
                    File.ReadAllText($"FormatInfo\\{_ortdpWrapper.GameStatus.CurrentStatus}FormatConfig.ini") : "";
               
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
        }
    }
}