using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InfoReaderPlugin.Command.Tools;
using Newtonsoft.Json;
using osuTools.MD5Tools;

namespace InfoReaderPlugin
{
    public class FileScanner
    {
        private readonly List<UpdateFileInfo> _infos = new List<UpdateFileInfo>();
        private static readonly MD5CryptoServiceProvider Md5Calculator = new MD5CryptoServiceProvider();
        private string _path;
        public UpdateFileInfo[] FileInfos => _infos.ToArray();
        public FileScanner(string path, SearchOption option = SearchOption.AllDirectories)
        {
            _path = path;
            string[] files = Directory.GetFiles(path, "*", option);
            foreach (var file in files)
            {
                _infos.Add(ProcessFile(file));
            }
        }
        string CalcMd5(byte[] bytes)
        {
            if (bytes is null || bytes.Length == 0)
                throw new ArgumentException();
            byte[] rslt = Md5Calculator.ComputeHash(bytes);
            return MD5String.GetString(rslt);
        }

        UpdateFileInfo ProcessFile(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                throw new InvalidOperationException();
            string relativeDir = Path.Combine(".", Path.GetDirectoryName(file.Replace(_path, "")) ?? throw new ArgumentException("Is not a directory"));
            
            string friendlyName = Path.GetFileNameWithoutExtension(file);
            
            
            string ext = Path.GetExtension(file);
            if (ext == ".dll" || ext == ".exe")
                ext = ".txt";
            string serverDir = relativeDir.Replace('\\','/');
            if (serverDir.StartsWith("/"))
                serverDir = serverDir.Substring(1);
            string fileName = Path.Combine(serverDir, friendlyName + ext).Replace('\\', '/');
            if (fileName.StartsWith("/") || fileName.StartsWith("\\"))
                fileName = fileName.Substring(1);

            string md5Hash = CalcMd5(File.ReadAllBytes(file));
            string downloadPath = Path.Combine(relativeDir,Path.GetFileName(file)).Replace("\\","/");
            if (downloadPath.StartsWith("/") || downloadPath.StartsWith("\\"))
                downloadPath = downloadPath.Substring(1);
            return new UpdateFileInfo(friendlyName, fileName, downloadPath, md5Hash);
        }
        public string ToUpdateFilesJson(PluginVersion version)
        {
            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer) {Formatting = Formatting.Indented, Indentation = 4};
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("version");
            jsonWriter.WriteValue(version.ToString());
            jsonWriter.WritePropertyName("files");
            jsonWriter.WriteStartArray();
            foreach (var fileInfo in _infos)
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("FriendlyName");
                jsonWriter.WriteValue(fileInfo.FriendlyName);
                jsonWriter.WritePropertyName("FileName");
                jsonWriter.WriteValue(fileInfo.FileName);
                jsonWriter.WritePropertyName("DownloadPath");
                jsonWriter.WriteValue(fileInfo.FileName);
                jsonWriter.WritePropertyName("Md5Hash");
                jsonWriter.WriteValue(fileInfo.FileName);

            }
            jsonWriter.WriteEnd();
            jsonWriter.WriteEnd();
            string json = writer.ToString();
            jsonWriter.Close();
            writer.Close();
            return json;
        }

        public List<string> ToSqlInsert(PluginVersion version, DatabaseInfo dbInfo)
        {
            StringBuilder builder = new StringBuilder();
            string verStr = dbInfo.KeywordToNameStart + dbInfo.TableFullName + dbInfo.KeywordToNameEnd;
            List<string> sqlStatments = new List<string>();
            int verId = dbInfo.GetVersionId(version.ToString());
            if (verId == -1)
                throw new ArgumentException("Can not get version id by gave string.");
            string[] columns = dbInfo.Columns.Split(',');
            if(columns[0] != "VersionId")
                throw new ArgumentException("First column must be \"VersioId\"");
            foreach (var updateFileInfo in FileInfos)
            {
                string format =
                    "INSERT INTO {0}{2}{1} ({0}VersionId{1},{0}FriendlyName{1},{0}FileName{1},{0}DownloadPath{1},{0}Md5Hash{1}) VALUES ({3},'{4}','{5}','{6}','{7}');";
                string sqlStatement =
                    string.Format(format, dbInfo.KeywordToNameStart, dbInfo.KeywordToNameEnd,dbInfo.TableFullName,verId,updateFileInfo.FriendlyName
                        ,updateFileInfo.FileName,updateFileInfo.DownloadPath,updateFileInfo.Md5);
                sqlStatments.Add(sqlStatement);
            }
            return sqlStatments;
        }
    }
    
}
