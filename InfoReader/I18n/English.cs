using System.Collections.Generic;

namespace InfoReaderPlugin.I18n
{
    public class English : LanguageFile
    {
        internal English() : base("en-us")
        {
            SQLiteLanguageDatabase tmpDatabase = new SQLiteLanguageDatabase();
            FileWriter = FileUpdater = tmpDatabase;
            FileReader = tmpDatabase;
        }
    }
}