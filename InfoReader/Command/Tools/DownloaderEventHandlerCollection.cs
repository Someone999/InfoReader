

namespace InfoReaderPlugin.Command.Tools
{
    public class DownloaderEventHandlerCollection
    {
        public DownloadFailedEventHandler FailedEventHandler { get; set; }
        public DownloadCompletedEventHandler CompletedEventHandler { get; set; }
        public DownloadProgressChangedEventHandler DownloadProgressChangedEventHandler { get; set; }
    }
}
