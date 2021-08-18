using System;
using System.Collections.Generic;

namespace InfoReaderPlugin
{
    public class PluginVersion:IEqualityComparer<PluginVersion>
    {
        public int Major { get; internal set; }
        public int Minor { get; internal set; }
        public int Additional { get; internal set; }
        public PluginVersion(string verstr)
        {
            if (string.IsNullOrEmpty(verstr) || string.IsNullOrWhiteSpace(verstr))
            {
                throw new ArgumentNullException(nameof(verstr),@"Version can not be null or empty");
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
}