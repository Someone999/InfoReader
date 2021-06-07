namespace InfoReaderPlugin.I18n
{
    public interface ILanguageFileWriter
    {
        void Write(string languageId,string key,string value);
        void Append(string languageId, string key, string value);
        void Delete(string languageId, string key, string value);
        void Clear(string languageId);
    }
}
