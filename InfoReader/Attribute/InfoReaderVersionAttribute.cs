namespace InfoReaderPlugin.Attribute
{
    public class InfoReaderVersionAttribute : System.Attribute
    {
        public InfoReaderVersionAttribute(string ver)
        {
            Version = new PluginVersion(ver);
        }
        public PluginVersion Version { get; }
    }
}