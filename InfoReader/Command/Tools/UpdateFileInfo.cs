using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using InfoReaderPlugin.Command.CommandClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfoReaderPlugin.Command.Tools
{
    public class UpdateFileInfo
    {
        public static UpdateFileInfo[] GetFiles(PluginVersion pluginVersion)
        {
            List<UpdateFileInfo> files = new List<UpdateFileInfo>();
            string url = string.Format(InfoReaderUrl.FileInfo, pluginVersion);
            string fileInfo = WebHelper.GetWebPageContent(url, timeout: 3000);
            return GetFilesFromJson(pluginVersion,(JObject)JsonConvert.DeserializeObject(fileInfo));
        }

        public string FriendlyName { get; internal set; }
        public string FileName { get; internal set; }
        public string DownloadPath { get; internal set; }
        public string Md5 { get; internal set; }

        public UpdateFileInfo(JObject jobj)
        {
            if (jobj != null)
            {
                FriendlyName = jobj["FriendlyName"].ToString();
                FileName = jobj["FileName"].ToString();
                DownloadPath = jobj["DownloadPath"].ToString();
                Md5 = jobj["Md5Hash"].ToString();
            }
        }

        public static UpdateFileInfo[] GetFilesFromJson(PluginVersion verifyVersion,JObject json)
        {
            var version = json?["version"];
            var files = json?["files"];
            if (version is null || files is null)
                throw new InvalidOperationException();
            PluginVersion ver = new PluginVersion(version.ToString());
            if (ver != verifyVersion)
                throw new ArgumentException("The requsted version doesn't match the version in the file.");
            return (from JObject jObj in files select new UpdateFileInfo(jObj)).ToArray();
        }
        public UpdateFileInfo(string friendlyName, string fileName,string downloadPath,string md5Hash)
        {
            FileName = fileName;
            FriendlyName = friendlyName;
            DownloadPath = downloadPath;
            Md5 = md5Hash;
        }

        public void Download(PluginVersion pluginVersion,Downloader downloader)
        {
            downloader = downloader ?? new Downloader(WebHelper.Combine(Update.PluginServerUrl));
            downloader.Download(this,pluginVersion);
        }
        public bool IsValid() =>
            !string.IsNullOrEmpty(FriendlyName) && !string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(DownloadPath) && !string.IsNullOrEmpty(Md5) &&
            !string.IsNullOrWhiteSpace(FriendlyName) && !string.IsNullOrWhiteSpace(FileName) && !string.IsNullOrWhiteSpace(DownloadPath) && !string.IsNullOrWhiteSpace(Md5);
    }
}
