using System.Collections.Generic;

namespace InfoReaderPlugin.I18n
{
    public class Chinese : LanguageFile
    {
        internal Chinese() : base("zh-cn")
        {
            SqliteLanguageDatabase tmpDatabase = new SqliteLanguageDatabase("InfoReader.db");
            FileWriter = FileUpdater = tmpDatabase;
            FileReader = tmpDatabase;

        }
    }
}