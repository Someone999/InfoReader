using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfoReaderPlugin
{
    public class SqlServerDatabaseInfo : DatabaseInfo
    {
        public override int GetVersionId(string versionString)
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://archaring.xyz/plugin/latestversion");
            Stream s = request.GetResponse().GetResponseStream() ?? new MemoryStream();
            using (StreamReader reader = new StreamReader(s))
            {
                JToken token = (JToken)JsonConvert.DeserializeObject(reader.ReadToEnd());
                return token?["version_id"]?.ToObject<int>() ?? -1;
            }
        }
    }
}