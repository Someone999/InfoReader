namespace InfoReaderPlugin.I18n
{
    public interface ILanguageFileReader
    {
        string Read(string languageId,string key);
        LanguageElement[] ReadAll(string languageId);
    }
}
