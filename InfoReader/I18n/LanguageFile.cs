using System.Collections.Generic;

namespace InfoReaderPlugin.I18n
{
    public abstract class LanguageFile:NI18n
    {
        public List<LanguageElement> LanguageElements { get; protected set; } = new List<LanguageElement>();
        public ILanguageFileReader FileReader { get; set; }
        public ILanguageFileWriter FileWriter { get; set; }
        public ILanguageFileUpdater FileUpdater { get; set; }
        public virtual void InitializeOperator(string file = "")
        {
            LanguageElements = new List<LanguageElement>(FileReader.ReadAll(LanguageId));
        }
        protected LanguageFile(string languageId,ILanguageFileReader reader = null, ILanguageFileWriter writer = null,
            ILanguageFileUpdater updater = null)
        {
            LanguageId = languageId;
            FileReader = reader;
            FileWriter = writer;
            FileUpdater = updater;
        }
    }
}
