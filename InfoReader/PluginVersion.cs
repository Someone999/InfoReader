using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using InfoReaderPlugin.Attribute;
using InfoReaderPlugin.Command.CommandClasses;
using InfoReaderPlugin.Command.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfoReaderPlugin
{
    [Flags]
    public enum AvailabilityObject
    {
        Changelog = 1,
        Files
    }
    public class PluginVersion : IEqualityComparer<PluginVersion>
    {
        public int Major { get; internal set; }
        public int Minor { get; internal set; }
        public int Additional { get; internal set; }

        public PluginVersion(string verstr)
        {
            if (string.IsNullOrEmpty(verstr) || string.IsNullOrWhiteSpace(verstr))
            {
                throw new ArgumentNullException(nameof(verstr), @"Version can not be null or empty");
            }

            Major = 0;
            Minor = 0;
            Additional = 0;
            string[] vers = verstr.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
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
                if (!r) throw new ArgumentException("Format of version is wrong。");
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
        public static bool operator <=(PluginVersion a, PluginVersion b) => a == b || a < b;
        public static bool operator >=(PluginVersion a, PluginVersion b) => a == b || a > b;
        public static bool operator ==(PluginVersion a, PluginVersion b)
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

        public static PluginVersion CurrentVersion
        {
            get
            {
                Type t = typeof(InfoReader);
                var attrs = t.GetCustomAttributes(false);
                foreach (var attr in attrs)
                {
                    if (attr is InfoReaderVersionAttribute attribute)
                    {
                        return attribute.Version;
                    }
                }
                return new PluginVersion("-1.0.0");
            }
        }

        public static PluginVersion LatestVersion
        {
            get
            {
                string version = WebHelper.GetWebPageContent(InfoReaderUrl.LatestVersion);
                if (string.IsNullOrEmpty(version))
                    return CurrentVersion;
                JObject verObj = (JObject)JsonConvert.DeserializeObject(version);
                return new PluginVersion(verObj?["VersionString"]?.ToString() ?? CurrentVersion.ToString());
            }
        }


        public Dictionary<AvailabilityObject,bool> GetAvailability(AvailabilityObject toQuery)
        {
            Dictionary<AvailabilityObject, bool> dict = new Dictionary<AvailabilityObject, bool>();
            string url = string.Format(InfoReaderUrl.AvaliabilityInfo, this, (int)toQuery);
            var content = WebHelper.GetWebPageContent(url);
            JObject jobj = (JObject)JsonConvert.DeserializeObject(content);
            if (jobj is null)
            {
                return null;
            }

            bool? changelogAva = jobj["Changelog"]?.ToObject<bool>();
            bool? filesAva = jobj["Files"]?.ToObject<bool>();
            if(changelogAva.HasValue)
                dict.Add(AvailabilityObject.Changelog,changelogAva.Value);
            if (filesAva.HasValue)
                dict.Add(AvailabilityObject.Files, filesAva.Value);
            return dict;
        }

        public string GetChangelog()
        {
            if (!GetAvailability(AvailabilityObject.Changelog)[AvailabilityObject.Changelog])
                return "This version has no changelog.";
            string url = string.Format(InfoReaderUrl.Changelog, this);
            HttpWebRequest request = WebRequest.CreateHttp(url);
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream() ??
                                                          throw new Exception("Fail to get changelog.")))
            {
                string changelog = reader.ReadToEnd();
                return changelog;
            }
            
        }

    }
}