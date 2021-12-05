using System;
using System.IO;
using System.Linq;
using System.Text;
using InfoReaderPlugin.I18n;
using osuTools.Beatmaps;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    class FileGetter:ICommandProcessor
    {

        void GetOsz(InfoReader instance)
        {
            var ortdpWrapper = instance.GetOrtdp();
            Win32Class.SevenZip zip = new Win32Class.SevenZip { SevenZipDirectory = "..\\pg\\7-zip\\7za.exe" };
            StringBuilder tmp = new StringBuilder(ortdpWrapper.FullPath);
            string basicinfo = $"{ortdpWrapper.Artist} - {ortdpWrapper.Title}";
            string pattern = "[" + new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars()) + ":*]";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);
            Win32Class.SevenZip.Native.SevenZipArgs arg = new Win32Class.SevenZip.Native.SevenZipArgs
            {
                FileType = Win32Class.SevenZip.Native.ArchieveType.Zip
            };
            string newdir = reg.Replace($"{ortdpWrapper.Artist} - {ortdpWrapper.Title}", " ");
            arg.ArchieveFileName = "\"" + instance.Setting.OszDir + "\\" + ortdpWrapper.Beatmap.BeatmapSetId + " " + newdir + ".osz\"";
            if (File.Exists(arg.ArchieveFileName.Trim('\"')))
            {
                IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_OSZEXISTED"));
                File.Delete(arg.ArchieveFileName.Trim('\"'));
            }
            arg.OperationType = Win32Class.SevenZip.Native.Operation.Add;
            if (!instance.Setting.OszDir.ToString().EndsWith("\\"))
            {
                arg.Files = "\"" + ortdpWrapper.Beatmap.BeatmapFolder + "\\*.*" + "\"";
            }
            else
            {
                arg.Files = "\"" + ortdpWrapper.Beatmap.BeatmapFolder + "\"";
            }
            var sta = Win32Class.SevenZip.Native.Run7zCmd(zip.SevenZipDirectory, arg);
            if (sta == Win32Class.SevenZip.Native.SevenZipStateCode.Success) IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_COMPRESSED"));
            if (sta == Win32Class.SevenZip.Native.SevenZipStateCode.FaltalError)
                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_7ZSTATUS_FATALERR"), ConsoleColor.Red);
            if (sta == Win32Class.SevenZip.Native.SevenZipStateCode.OutOfMemory)
                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_7ZSTATUS_OUTOFMEMORY"), ConsoleColor.Red);
            if (sta == Win32Class.SevenZip.Native.SevenZipStateCode.Canceled)
                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_7ZSTATUS_OPERATIONCANCELED"), ConsoleColor.Red);
            if (sta == Win32Class.SevenZip.Native.SevenZipStateCode.CommandLineError)
                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_7ZSTATUS_COMMANDLINEERR"), ConsoleColor.Red);
            if (sta == Win32Class.SevenZip.Native.SevenZipStateCode.Warning)
                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_7ZSTATUS_WARNINGS"), ConsoleColor.Yellow);
        }

        public string MainCommand => "get";
        public bool AutoCatch { get; set; } = true;
        public bool OnUnhandledException(InfoReader instance, Exception exception)
        {
            return false;
        }
        public string GetHelp() => NI18n.GetLanguageElement("LANG_HELP_FILEGETTER");
        string GetValidDir(string dir)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToArray();
            StringBuilder validDir = new StringBuilder();
            foreach (var s in dir)
            {
                validDir.Append(!invalidChars.Contains(s) ? s : ' ');
            }
            return validDir.ToString();
        }
        public bool Process(InfoReader instance, CommandParser parser)
        {
            Beatmap currentBeatmap = instance.GetOrtdp().Beatmap;
            if (parser.Arguments[0] == "au")
            {
                if (currentBeatmap is null || string.IsNullOrEmpty(currentBeatmap.AudioFileName) || !File.Exists(currentBeatmap.FullAudioFileName))
                    return true;
                string ext = Path.GetExtension(currentBeatmap.FullAudioFileName);
                string targetPath = GetName(instance.Setting.DefaultMusicCopyingDirectory, currentBeatmap,ext);
                if (!File.Exists(targetPath))
                    File.Copy(currentBeatmap.FullAudioFileName ?? throw new InvalidOperationException(),targetPath);
                IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_COPYSUCCESS"));
            }
            else if (parser.Arguments[0] == "vi")
            {
                if (currentBeatmap is null || !currentBeatmap.HasVideo || string.IsNullOrEmpty(currentBeatmap.VideoFileName) || !File.Exists(currentBeatmap.FullVideoFileName))
                    return true;
                string ext = Path.GetExtension(currentBeatmap.VideoFileName);
                string targetPath = GetName(instance.Setting.DefaultMusicCopyingDirectory, currentBeatmap, ext);
                if (!File.Exists(targetPath))
                    File.Copy(currentBeatmap.FullVideoFileName ?? throw new InvalidOperationException(), targetPath);
                IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_COPYSUCCESS"));
            }
            else if (parser.Arguments[0] == "bg")
            {
                if (currentBeatmap is null || string.IsNullOrEmpty(currentBeatmap.BackgroundFileName) || !File.Exists(currentBeatmap.FullBackgroundFileName))
                    return true;
                string ext = Path.GetExtension(currentBeatmap.BackgroundFileName);
                string targetPath = GetName(instance.Setting.DefaultBackgroundCopyingDirectory, currentBeatmap, ext);
                if (!File.Exists(targetPath))
                    File.Copy(currentBeatmap.FullBackgroundFileName ?? throw new InvalidOperationException(), targetPath);
                IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_COPYSUCCESS"));
            }
            else if (parser.Arguments[0] == "osz")
            {
                GetOsz(instance);
            }
            else
            {
                IO.CurrentIO.Write(GetHelp());
            }
            return true;
        }
        void TryCreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        string GetName(string folder,IBeatmap beatmap,string extension)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentException("Folder name can not be null or empty.");
            if (beatmap is null)
                throw new ArgumentNullException(nameof(beatmap),"Beatmap can not be null.");
            string beatmapFolder = $"{beatmap.Artist} - {beatmap.Title}";
            folder = Path.Combine(folder, beatmapFolder);
            TryCreateDirectory(folder);
            string orignalName = $"{beatmap.Artist} - {beatmap.Title} [{beatmap.Version}]";
            if (folder.Length + orignalName.Length > 248)
                orignalName = $"{beatmap.Title} [{beatmap.Version}]";
            if(folder.Length + orignalName.Length > 248)
                orignalName = $"{beatmap.Title}";
            if (folder.Length + orignalName.Length > 248)
            {
                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_ERR_PATHTOOLONG"),ConsoleColor.Red);
                return null;
            }
            if (!extension.StartsWith("."))
                extension = "." + extension;
            orignalName = GetValidDir(orignalName);
            return Path.Combine(folder, orignalName) + extension;
        }
    }
}
