namespace InfoReaderPlugin.I18n
{
    public class LanguageElement
    {
        public string Key { get; }
        public string Value { get; }
        public LanguageElement(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
