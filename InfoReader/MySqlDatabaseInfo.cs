using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReaderPlugin
{
    public class MySqlDatabaseInfo:DatabaseInfo
    {
        public override char KeywordToNameEnd => '`';
        public override char KeywordToNameStart => '`';
        public override string TableFullName { get; set; } = "PluginDatabase.PluginFileInfo";

        public override int GetVersionId(string versionString)
        {
            throw new NotImplementedException();
        }
    }
}
