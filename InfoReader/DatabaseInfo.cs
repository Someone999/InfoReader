namespace InfoReaderPlugin
{
    public abstract class DatabaseInfo
    {
        /// <summary>
        /// Must be VersionId,FriendlyName,FileName,DownloadPath,Md5Hash
        /// </summary>
        public string Columns => "VersionId,FriendlyName,FileName,DownloadPath,Md5Hash";

        public virtual string TableFullName { get; set; } = "PluginFileInfo";

        public virtual char KeywordToNameStart => '[';
        public virtual char KeywordToNameEnd => ']';
        /// <summary>
        /// Return the version id of plugin when succed. Otherwise return -1.
        /// </summary>
        /// <param name="versionString"></param>
        /// <returns></returns>
        public abstract int GetVersionId(string versionString);

    }
}