using System.Collections.Generic;

namespace InfoReaderPlugin.I18n
{
    public class Japanese : LanguageFile
    {
        internal Japanese():base("ja-jp")
        {
            SQLiteLanguageDatabase tmpDatabase = new SQLiteLanguageDatabase();
            FileWriter = FileUpdater = tmpDatabase;
            FileReader = tmpDatabase;
        }
    }
}