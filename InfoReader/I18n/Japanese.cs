using System.Collections.Generic;

namespace InfoReaderPlugin.I18n
{
    public class Japanese : LanguageFile
    {
        internal Japanese():base("ja-jp")
        {
            SqliteLanguageDatabase tmpDatabase = new SqliteLanguageDatabase();
            FileWriter = FileUpdater = tmpDatabase;
            FileReader = tmpDatabase;
        }
    }
}