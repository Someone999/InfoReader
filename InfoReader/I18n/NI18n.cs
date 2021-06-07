using System;
using System.Linq;
using System.Reflection;

namespace InfoReaderPlugin.I18n
{
    public abstract class NI18n
    {
        private static LanguageFile _languageFile;
        public string LanguageId { get; protected set; }
        public static LanguageFile GetCurrentLanguage()
        {
            if (_languageFile is null)
            {
                var languages = from type in Assembly.GetExecutingAssembly().GetTypes()
                    where type.BaseType == typeof(LanguageFile)
                    select type;
                foreach (var lang in languages)
                {
                    var constructor = lang.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null) ??
                                      lang.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof(string)}, null);
                    if (constructor is null) continue;
                    _languageFile = (LanguageFile)constructor.Invoke(new object[0]);
                    if (_languageFile.LanguageId == InfoReader.SyncLang)
                    {
                        break;
                    }
                }
            }

            return _languageFile;
        }
        public static string GetLanguageElement(string key)
        {
            if (_languageFile is null)
                GetCurrentLanguage();
            return _languageFile?.LanguageElements?.FirstOrDefault(element => element.Key == key)?.Value ?? "null";
        }
    }
}