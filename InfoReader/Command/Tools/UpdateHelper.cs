using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReaderPlugin.Command.CommandClasses;
using InfoReaderPlugin.I18n;
using Sync.Tools;

namespace InfoReaderPlugin.Command.Tools
{
    public static class UpdateHelper
    {
        public static bool IsLatestVersion() => PluginVersion.CurrentVersion >= PluginVersion.LatestVersion;

        public static void DownloadFiles(PluginVersion version)
        {
            
            Downloader downloader = new Downloader(Update.PluginServerUrl);
            var files = UpdateFileInfo.GetFiles(version);
            foreach (var updateFileInfo in files)
            {
                IO.CurrentIO.Write(string.Format(NI18n.GetLanguageElement("LANG_INFO_DOWNLOAD"),updateFileInfo.FriendlyName));
                updateFileInfo.Download(version, downloader);
                IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_DOWNLOADED"));
            }
        }
    }
}
