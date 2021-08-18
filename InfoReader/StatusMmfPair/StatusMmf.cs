using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using osuTools.Game;
using osuTools.Game.Modes;

namespace InfoReaderPlugin.StatusMmfPair
{
    public abstract class StatusMmf
    {
        protected StatusMmf(string mmfName)
        {
            if (mmfName is null)
                return;
            MappedFile = MemoryMappedFile.CreateOrOpen(mmfName,4096);
        }

        public bool EnableOutput { get; set; } = true;
        public virtual OsuGameStatus TargetStatus => OsuGameStatus.Unkonwn;
        public MemoryMappedFile MappedFile { get; }
        public Stream StreamOfMappedFile => MappedFile?.CreateViewStream();

        public virtual string ConfigFilePath => $"FormatInfo\\{TargetStatus}FormatConfig.ini";
        private string _formatString;
        public virtual string FormatString => EnableOutput ? _formatString = File.ReadAllText(ConfigFilePath) : string.Empty;

        public virtual void OnMmfChanged(StatusMmf currentMmf, StatusMmf newMmf)
        {
            currentMmf.EnableOutput = false;
            for (int i = 0; i < 10; i++)
            {
                currentMmf.StreamOfMappedFile.Write(new byte[1024], 0, 1024);
                currentMmf.StreamOfMappedFile.Flush();
            }
            newMmf.EnableOutput = true;
        }
       

        private static Dictionary<OsuGameStatus, StatusMmf> _statusMmf;

        public static StatusMmf GetMmfByStatus(OsuGameStatus status)
        {
            if (_statusMmf is null)
            {
                _statusMmf = new Dictionary<OsuGameStatus, StatusMmf>();
                Type[] t = typeof(StatusMmf).Assembly.GetTypes();
                foreach (var type in t)
                {
                    if (type.BaseType == typeof(StatusMmf))
                    {
                        var constructor = type.GetConstructor(new Type[0]);
                        StatusMmf statusMmf = constructor?.Invoke(new object[0]) as StatusMmf;
                        if (statusMmf is null)
                            continue;
                        _statusMmf.Add(statusMmf.TargetStatus, statusMmf);
                    }
                }

                _statusMmf.TryGetValue(status, out var outerMmf);
                return outerMmf;
            }
            if (_statusMmf.ContainsKey(status))
                return _statusMmf[status];
            return null;
        }
    }
}