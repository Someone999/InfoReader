using System.Collections.Generic;

namespace InfoReaderPlugin.I18n
{
    public class English : LanguageFile
    {
        internal English() : base("en-us")
        {
            SqliteLanguageDatabase tmpDatabase = new SqliteLanguageDatabase();
            FileWriter = FileUpdater = tmpDatabase;
            FileReader = tmpDatabase;
        }
    }
}