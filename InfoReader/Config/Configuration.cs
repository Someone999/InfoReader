using System;
using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;
using System.IO;
using System.Windows;
using InfoReaderPlugin.Config.Converters;
using InfoReaderPlugin.I18n;

namespace InfoReaderPlugin
{
    public class Setting : IConfigurable
    {
        private readonly BoolConfigConverter _debugModeConverter = new BoolConfigConverter(false);
        private readonly BoolConfigConverter _autoupdateConverter = new BoolConfigConverter(true);
        internal string encoding = "utf-8";
        public ConfigurationElement ApiKey { get; set; } = "";
        [Bool]
        public ConfigurationElement DebugMode { get => _debugModeConverter.ConvertToString(); set => _debugModeConverter.Convert(value); }
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
        [Bool]
        public ConfigurationElement AutoUpdate
        {
            get => _autoupdateConverter.ConvertToString();
            set => _autoupdateConverter.Convert(value);
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