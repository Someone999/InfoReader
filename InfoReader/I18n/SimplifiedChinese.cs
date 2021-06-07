using System.Collections.Generic;

namespace InfoReaderPlugin.I18n
{
    public class Chinese : LanguageFile
    {
        internal Chinese() : base("zh-cn")
        {
            SQLiteLanguageDatabase tmpDatabase = new SQLiteLanguageDatabase("InfoReader.db");
            FileWriter = FileUpdater = tmpDatabase;
            FileReader = tmpDatabase;

        }
    }
}