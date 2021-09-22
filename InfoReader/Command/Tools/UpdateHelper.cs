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

        public static void DownloadFiles(PluginVersion version ,out int fileCount,DownloaderEventHandlerCollection handlers = null)
        {
            bool failed = false;
            DownloadFailedEventHandler failIsNull = (fileName, err, exp) => failed = true;
            DownloadFailedEventHandler failedIsNotNull = (fileName, err, exp) =>
            {
                failed = true;
                handlers.FailedEventHandler(fileName, err, exp);
            };
            Downloader downloader = new Downloader(InfoReaderUrl.BaseUrl /*This argument is NOT used here*/);
            downloader.OnProgressChanged += handlers?.DownloadProgressChangedEventHandler;
            downloader.OnDownloadFailed += handlers?.FailedEventHandler is null ? failIsNull : failedIsNotNull;
            downloader.OnDownloadCompleted += handlers?.CompletedEventHandler;

            var files = UpdateFileInfo.GetFiles(version);
            fileCount = files.Length;
            foreach (var updateFileInfo in files)
            {
                if (failed)
                {
                    break;
                }
                updateFileInfo.Download(version, downloader);
            }
        }
    }
}
