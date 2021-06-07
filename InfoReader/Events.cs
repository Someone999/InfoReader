using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfoReaderPlugin.I18n;
using InfoReaderPlugin.MemoryMapWriter;
using osuTools;
using osuTools.Attributes;
using osuTools.Beatmaps;
using osuTools.OsuDB;

namespace InfoReaderPlugin
{

    public class PEvent : IPluginEvent
    {
        public PEvent(string msg)
        {
            Message = msg;
        }
        public string Message { get; }
    }
    public class PluginVersion:IEqualityComparer<PluginVersion>
    {
        public int Major { get; internal set; }
        public int Minor { get; internal set; }
        public int Additional { get; internal set; }
        public PluginVersion(string verstr)
        {
            if (string.IsNullOrEmpty(verstr) || string.IsNullOrWhiteSpace(verstr))
            {
                throw new ArgumentNullException(nameof(verstr),@"版本号不能为空。");
            }
            Major = 0;
            Minor = 0;
            Additional = 0;
            string[] vers = verstr.Split(new[] { '.' },StringSplitOptions.RemoveEmptyEntries);
            if (vers.Length == 3)
            {
                Major = int.Parse(vers[0]);
                Minor = int.Parse(vers[1]);
                Additional = int.Parse(vers[2]);
            }
            if (vers.Length == 2)
            {
                Major = 0;
                Minor = int.Parse(vers[0]);
                Additional = int.Parse(vers[1]);
            }

            if (vers.Length <= 1)
            {
                Major = 0;
                Minor = 0;
                double b = 0;
                bool r = double.TryParse(verstr, out b);
                if (!r) throw new ArgumentException("版本号的格式不正确。");
                Additional = (int)b;
            }       
        }
        public static bool operator >(PluginVersion a, PluginVersion b)
        {
            if (a is null || b is null)
                return false;
            if (a.Major > b.Major) return true;
            if (a.Minor > b.Minor) return true;
            return a.Additional > b.Additional;
        }
        public static bool operator <(PluginVersion a, PluginVersion b)
        {
            if (a is null || b is null)
                return false;
            if (a.Major < b.Major) return true;
            if (a.Minor < b.Minor) return true;
            return a.Additional < b.Additional;
        }
        public static bool operator==(PluginVersion a, PluginVersion b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Major == b.Major && a.Minor == b.Minor && a.Additional == b.Additional;
        }
        public static bool operator !=(PluginVersion a, PluginVersion b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;
            
            return a.Major != b.Major || a.Minor != b.Minor || a.Additional != b.Additional;
        }
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Additional}";
        }
        public bool Equals(PluginVersion a, PluginVersion b) => a == b;
        public int GetHashCode(PluginVersion a) => a.Additional * 10 + a.Minor * 102 + a.Major * 1024 + 1354691254;
        public override bool Equals(object obj)
        {
            if (obj is PluginVersion p)
                return Equals(this, p);
            return false;
        }
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }
        public static readonly PluginVersion OldestVersion = new PluginVersion("0.0.20200918");
    }
    public class InfoReaderVersionAttribute : Attribute
    {
        public InfoReaderVersionAttribute(string ver)
        {
            Version = new PluginVersion(ver);
        }
        public PluginVersion Version { get; }
    }


    [InfoReaderVersion("1.0.16")]
    public partial class InfoReader
    {
        void Init(PluginEvents.InitPluginEvent e)
        {
        }
        void Loaded(PluginEvents.LoadCompleteEvent e)
        {
            var ortdp = getHoster().EnumPluings().FirstOrDefault(p => p.Name == "OsuRTDataProvider");
            var rtt = getHoster().EnumPluings().FirstOrDefault(p => p.Name == "RealTimePPDisplayer");
            _rawFormat = new StringBuilder();
            try
            {
                if (rtt is RealTimePPDisplayer.RealTimePPDisplayerPlugin r)
                {

                    _rtppdInfo = new RtppdInfo();
                    if (_rtppdInfo != null)
                    {
                        _ortdpWrapper = new osuTools.OrtdpWrapper.OrtdpWrapper(ortdp as OsuRTDataProvider.OsuRTDataProviderPlugin, r, _rtppdInfo);
                        IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_INITSUC"));
                    }
                }
                IO.CurrentIO.Write(Environment.CurrentDirectory);
                var process = System.Diagnostics.Process.GetCurrentProcess();
                var curdir = process.MainModule?.FileName.Replace("Sync.exe", "plugins");
                Environment.CurrentDirectory = curdir??"";
                GetAvaProperties(_ortdpWrapper);
            }
            catch (NullReferenceException ex)
            {
                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_INITFAILED") + ex.ToString(), ConsoleColor.Red);
            }
            ThreadPool.QueueUserWorkItem(state => ConigFileWatcher());
            ThreadPool.QueueUserWorkItem(state => RefreshMmf());
        }
        private void GetAvaProperties(object obj)
        {
            
            if (obj is null)
                return;
           
            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    List<AliasAttribute> aliasAttrs =
                        new List<AliasAttribute>(property.GetCustomAttributes<AliasAttribute>());
                    List<AvailableVariableAttribute> avaAttrs =
                        new List<AvailableVariableAttribute>(property
                            .GetCustomAttributes<AvailableVariableAttribute>());
                    if (aliasAttrs.Count != 0 || avaAttrs.Count != 0)
                    {
                        aliasAttrs.ForEach((attribute) =>
                        {
                            try
                            {
                                if(VariablePropertyInfos.All(info => info.Alias != attribute.Alias))
                                    VariablePropertyInfos.Add(new VariablePropertyInfo(attribute.Alias,avaAttrs[0].VariableName,property,obj));
                            }
                            catch (Exception)
                            {
                                // ignore
                            }
                        });
                        avaAttrs.ForEach(attribute =>
                        {
                            try
                            {
                                if (!VariablePropertyInfos.Any((info => info.Alias == attribute.VariableName)))
                                    VariablePropertyInfos.Add(new VariablePropertyInfo(attribute.VariableName,attribute.VariableName, property, obj));
                                if(VariablePropertyInfos.All(info => info.Alias != property.Name))
                                    VariablePropertyInfos.Add(new VariablePropertyInfo(attribute.VariableName,attribute.VariableName, property, obj));
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        });
                        if (avaAttrs.Count > 0)
                            GetAvaProperties(property.GetValue(obj));

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
    }
}