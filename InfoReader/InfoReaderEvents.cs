using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using InfoReaderPlugin.Attribute;
using InfoReaderPlugin.I18n;
using osuTools.Attributes;
using Sync.Plugins;
using Sync.Tools;

namespace InfoReaderPlugin
{
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
                        {
                            try
                            {
                                GetAvaProperties(property.GetValue(obj));
                            }
                            catch (NullReferenceException)
                            {
                                //ignored
                            }
                            catch (ArgumentException)
                            {
                                //ignored
                            }
                            catch(TargetInvocationException)
                            {
                                //ignored
                            }

                            
                        }

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