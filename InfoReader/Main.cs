using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Sync.Plugins;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfoReaderPlugin.Command;
using InfoReaderPlugin.Command.CommandClasses;
using InfoReaderPlugin.Command.Tools;
using InfoReaderPlugin.I18n;
using InfoReaderPlugin.StatusMmfPair;
using osuTools.Game;
using osuTools.MD5Tools;
using osuTools.OrtdpWrapper;
using Sync.Tools;
using Timer = System.Windows.Forms.Timer;

namespace InfoReaderPlugin

{
    [SyncSoftRequirePlugin("RealTimePPDisplayerPlugin", "OsuRTDataProviderPlugin")]
    public partial class InfoReader : Sync.Plugins.Plugin
    {
        //osuTools.Online.OnlineBestResultCollection best = new osuTools.Online.OnlineBestResultCollection();
        string _fileFormat;
        internal static readonly List<VariablePropertyInfo> VariablePropertyInfos = new List<VariablePropertyInfo>();
        public OrtdpWrapper GetOrtdp() => _ortdpWrapper;
        public RtppdInfo GetRtppi() => _rtppdInfo;
        private Dictionary<string, string> _varDictionary = new Dictionary<string, string>();
        private System.Windows.Forms.Timer _timer = new Timer();
        public CommandProcessorCollection CommandProcessors { get; } = CommandProcessorCollection.GetInstance();
        public StatusMmf CurrentStatusMmf { get; private set; }
        private OsuGameStatus _lastMmfStatus = OsuGameStatus.Unkonwn;
        public Setting Setting { get; } = new Setting();
        RtppdInfo _rtppdInfo;
        OrtdpWrapper _ortdpWrapper;
        public const string PLUGIN_NAME = "InfoReader";
        public const string PLUGIN_AUTHOR = "Someone999";
        public static LanguageFile CurrentNi18N;
        public static string SyncLang { get; private set; }
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName
        );

        void ScanCommand()
        {
            Type[] t = GetType().Assembly.GetTypes();
            foreach (var type in t)
            {
                if (type.GetInterfaces().Any(i => i == typeof(ICommandProcessor)))
                {
                    if (type.GetConstructor(new Type[0])?.Invoke(new object[0]) is ICommandProcessor processor)
                        CommandProcessors.AddCommandProcessor(processor);
                }
            }
        }

        void InitializeLanguage()
        {
            CheckDatabase();
            StringBuilder builder = new StringBuilder(32);
            GetPrivateProfileString("Sync.DefaultConfiguration", "Language", "zh-cn", builder, 32, "..\\config.ini");
            SyncLang = builder.ToString();
            CurrentNi18N = NI18n.GetCurrentLanguage();
            CurrentNi18N.InitializeOperator();
        }
        public InfoReader() : base(PLUGIN_NAME, PLUGIN_AUTHOR)
        {
            InitializeLanguage();
            ScanCommand();
            CheckRtppd();
            /*if(!File.Exists("bass.dll"))
                File.WriteAllBytes("bass.dll",Resource1.bass);*/
            _outputInfoMap = MemoryMappedFile.CreateOrOpen("InfoReaderMmf",128);
            EventBus.BindEvent<PluginEvents.InitPluginEvent>(Init);
            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(Loaded);
            EventBus.BindEvent<PluginEvents.InitCommandEvent>(CommandInit);
            PluginConfigurationManager manager = new Sync.Tools.PluginConfigurationManager(this);
            manager.AddItem(Setting);
        }
        public override void OnExit()
        {
            if(File.Exists("bass.dll"))
                File.Delete("bass.dll");
        }

        private void CheckDatabase()
        {
            if(!File.Exists("InfoReader.db"))
                File.WriteAllBytes("InfoReader.db",Resource1.Database);
        }

        private void CheckRtppd()
        {
            if (File.Exists("RealTimePPDisplayer.dll"))
            {
                byte[] b = File.ReadAllBytes("RealTimePPDisplayer.dll");
                MD5 selfMd5 = MD5.Create(), fileMd5 = MD5.Create();
                selfMd5.ComputeHash(Resource1.ModifiedRealTimePPDisplayer);
                fileMd5.ComputeHash(b);
                if (MD5String.GetString(selfMd5) != MD5String.GetString(fileMd5))
                {
                    var dirInfo = Directory.CreateDirectory("..\\OrignalRealTimePPDisplayer\\");
                    IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_RTPPDREPLACED"));
                    if (File.Exists(Path.Combine(dirInfo.FullName, "RealTimePPDisplayer.dll")))
                        File.Delete(Path.Combine(dirInfo.FullName, "RealTimePPDisplayer.dll"));
                    File.Move("RealTimePPDisplayer.dll", Path.Combine(dirInfo.FullName, "RealTimePPDisplayer.dll"));
                    File.WriteAllBytes("RealTimePPDisplayer.dll", Resource1.ModifiedRealTimePPDisplayer);
                }

            }
            else
            {
                File.WriteAllBytes("RealTimePPDisplayer.dll", Resource1.ModifiedRealTimePPDisplayer);
            }
        }
    }
}