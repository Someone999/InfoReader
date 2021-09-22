using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfoReaderPlugin.Attribute;
using InfoReaderPlugin.Command.CommandClasses;
using InfoReaderPlugin.I18n;
using osuTools.Attributes;
using osuTools.OrtdpWrapper;
using Sync.Plugins;
using Sync.Tools;

namespace InfoReaderPlugin
{
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
                        _ortdpWrapper =
                            new OrtdpWrapper(ortdp as OsuRTDataProvider.OsuRTDataProviderPlugin,
                                r, _rtppdInfo);
                        IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_INITSUC"));
                    }
                }

                var process = System.Diagnostics.Process.GetCurrentProcess();
                var curdir = process.MainModule?.FileName.Replace("Sync.exe", "plugins");
                Environment.CurrentDirectory = curdir ?? "";
                GetAvaProp(_ortdpWrapper.GetType());
            }
            catch (NullReferenceException ex)
            {
                IO.CurrentIO.WriteColor(NI18n.GetLanguageElement("LANG_INFO_INITFAILED") + ex, ConsoleColor.Red);
            }

            ThreadPool.QueueUserWorkItem(state => ConigFileWatcher());
            ThreadPool.QueueUserWorkItem(state => RefreshMmf());
            Task.Run(() => Update.CheckUpdate());
        }


        static void GetAvaProp(Type t)
        {
            Assembly osuToolsAssembly = t.Assembly;
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            SpecialDictionary<PropertyInfo, AvailableVariableAttribute> avaVarAttrs =
                new SpecialDictionary<PropertyInfo, AvailableVariableAttribute>();
            properties = (from p in properties where 
                    avaVarAttrs.AddAndReturn(p,p.GetCustomAttribute<AvailableVariableAttribute>(),false).Value != null 
                    select p).ToArray();
            foreach (var propertyInfo in properties)
            {
                var avaAttr = avaVarAttrs[propertyInfo];
                if (propertyInfo.PropertyType.Assembly == osuToolsAssembly)
                {
                    GetAvaProp(propertyInfo.PropertyType);
                }
                VariablePropertyInfos.Add(new VariablePropertyInfo(avaAttr.VariableName,
                    avaAttr.VariableName, propertyInfo));
                if (!avaAttr.HasAlias) 
                    continue;
                foreach (var aliasAttribute in avaAttr.Alias)
                {
                    if (aliasAttribute == avaAttr .VariableName) 
                        continue;
                    VariablePropertyInfos.Add(new VariablePropertyInfo(aliasAttribute,
                        avaAttr.VariableName, propertyInfo));
                }
            }
        }
    }

    class SpecialDictionary<TKey,TValue> : Dictionary<TKey,TValue> where TValue: class
    {
        public KeyValuePair<TKey,TValue> AddAndReturn(TKey key,TValue value, bool addNullValue)
        {
            if(addNullValue || !(value is null))
               Add(key,value);
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}