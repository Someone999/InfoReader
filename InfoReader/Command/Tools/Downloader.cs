using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InfoReaderPlugin.I18n;
using osuTools.MD5Tools;
using Sync.Tools;

namespace InfoReaderPlugin.Command.Tools
{
    public class Downloader
    {
        private string _baseUrl;
        public HttpWebRequest Request { get; private set; }
        public delegate double ProgressChangedEventHandler(double progress);

        public event ProgressChangedEventHandler OnProgressChanged;

        static bool Verify(byte[] fileBytes, string md5) => string.IsNullOrEmpty(md5) ||
                                                            MD5String.GetString(MD5.Create().ComputeHash(fileBytes)) == md5;
        
        public Downloader(string baseUrl)
        {
            _baseUrl = WebHelper.UrlAutoComplete(baseUrl);
        }

        public void Download(string fileName,string localFileName = null,string md5 = null,int timeout = 100000)
        {
            try
            {
                string url = WebHelper.Combine(_baseUrl, fileName);
                localFileName = localFileName ?? fileName;
                string fileNameNew = $"{localFileName}_new";
                Request = WebRequest.CreateHttp(url);
                Request.Timeout = timeout;
                using (HttpWebResponse response = Request.GetResponse() as HttpWebResponse)
                {
                    HttpStatusCode? statusCode = response?.StatusCode;
                    if (statusCode.HasValue && statusCode.Value != HttpStatusCode.OK)
                    {
                        return;
                    }
                    var responseStream = response?.GetResponseStream() ?? throw new NullReferenceException();
                    int readLen;
                    double currentLen = 0, len = response.ContentLength;
                    byte[] buffer = new byte[8192];
                    MemoryStream tempStream = new MemoryStream();
                    while ((readLen = responseStream.Read(buffer, 0, 8192)) != 0)
                    {
                        currentLen += readLen;
                        OnProgressChanged?.Invoke(currentLen / len);
                        tempStream.Write(buffer, 0, readLen);
                    }

                    string dir = Path.GetDirectoryName(fileNameNew);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }
                    FileStream fstream = File.Create(fileNameNew);
                    if (!Verify(tempStream.ToArray(), md5))
                    {
                        fstream.Close();
                        File.Delete(fileNameNew);
                        IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_ERR_VERIFYFAILED"), ConsoleColor.Red);
                    }
                    else
                    {
                        fstream.Write(tempStream.ToArray(), 0, (int)tempStream.Length);
                        fstream.Close();
                        File.Delete(localFileName);
                        File.Move(fileNameNew, localFileName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(NI18n.GetLanguageElement("LANG_ERR_DOWNLOADFAILED"), "\n" + e);
                throw;
            }
            
        }

        public void Download(UpdateFileInfo updateFile, PluginVersion version, int timeout = 100000)
        {
            _baseUrl = string.Format(InfoReaderUrl.PluginDownloadUrl,version);
            Download(updateFile.FileName, Path.Combine("InfoReader",updateFile.DownloadPath), updateFile.Md5, timeout);
        }
    }
}
