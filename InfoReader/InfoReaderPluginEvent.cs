using Sync.Plugins;
using System.Threading.Tasks;
using osuTools;
using osuTools.Beatmaps;
using osuTools.OsuDB;

namespace InfoReaderPlugin
{

    public class InfoReaderPluginEvent : IPluginEvent
    {
        public InfoReaderPluginEvent(string msg)
        {
            Message = msg;
        }
        public string Message { get; }
    }
}