using System.IO;
using System.Net;
using InfoReaderPlugin.Command.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfoReaderPlugin
{
    public class SqlServerDatabaseInfo : DatabaseInfo
    {
        public override int GetVersionId(string versionString)
        {
            JToken token = (JToken)JsonConvert.DeserializeObject(WebHelper.GetWebPageContent(string.Format(InfoReaderUrl.Version,versionString), timeout: 5000));
            return token?["version_id"]?.ToObject<int?>() ?? -1;
        }
    }
}