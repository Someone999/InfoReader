using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;
using System.IO;
using System.Windows;
using InfoReaderPlugin.I18n;

namespace InfoReaderPlugin
{
    public class Setting : IConfigurable
    {
        bool _debug = false;
        internal string encoding = "utf-8";
        public ConfigurationElement ApiKey { get; set; } = "fa2748650422c84d59e0e1d5021340b6c418f62f";
        [Bool]
        public ConfigurationElement DebugMode { get => _debug.ToString(); set => _debug = bool.Parse(value); }
        [Path]
        public ConfigurationElement DefaultMusicCopyingDirectory { get; set; } = $"{System.Environment.CurrentDirectory}\\BeatmapMusic\\";
        [Path]
        public ConfigurationElement DefaultVideoCopyingDirectory { get; set; } = $"{System.Environment.CurrentDirectory}\\BeatmapVideo\\";
        [Path]
        public ConfigurationElement DefaultBackgroundCopyingDirectory { get; set; } = $"{System.Environment.CurrentDirectory}\\BeatmapBg\\";
        [Path]
        public ConfigurationElement GameDir { get; set; }
        [Path]
        public ConfigurationElement OszDir { get; set; } = $"{System.Environment.CurrentDirectory}\\oszFiles";
        [List(ValueList =new []{ "Ortdp", "OsuDb" })]
        public ConfigurationElement BeatmapReadMethod { get; set; } = "Ortdp";
        [String]
        public ConfigurationElement Encoding 
        { get=>encoding;
            set
            {
                if (value != "gb2312" && value != "gbk" && value!="big5")
                    encoding = value;
                else
                {
                    try
                    {
                        encoding = "utf-8";
                        string format = NI18n.GetLanguageElement("LANG_ERR_ENCODING");
                        MessageBox.Show(string.Format(format, value));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
        public void onConfigurationLoad()
        {
            if (!File.Exists(GameDir) || string.IsNullOrEmpty(GameDir))
                GameDir = "";
        }
        public void onConfigurationReload()
        {
            

        }
        public void onConfigurationSave()
        {

        }
    }
}