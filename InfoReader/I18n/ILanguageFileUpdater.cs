namespace InfoReaderPlugin.I18n
{
    public interface ILanguageFileUpdater: ILanguageFileWriter
    {
        bool NeedUpdate(string languageId);
    }
}
