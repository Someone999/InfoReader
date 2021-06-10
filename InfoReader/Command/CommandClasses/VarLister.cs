using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using InfoReaderPlugin.I18n;
using osuTools;
using osuTools.Attributes;
using osuTools.MD5Tools;
using Sync.Tools;

namespace InfoReaderPlugin.Command.CommandClasses
{
    public class VarLister:ICommandProcessor
    {
        public string MainCommand => "list";
        public bool AutoCatch { get; set; } = true;
        public bool OnUnhandledException(InfoReader instance, Exception exception)
        {
            return false;
        }
        public bool Process(InfoReader infoReader,CommandParser parser)
        {
            Type t = infoReader.GetOrtdp().GetType();
            PropertyInfo[] properties = t.GetProperties();
            StringBuilder b = new StringBuilder();
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<AvailableVariableAttribute>() is AvailableVariableAttribute attr)
                    b.AppendLine($"{attr.VariableName}   {NI18n.GetLanguageElement(attr.LanguageElementName)}");
                if (property.PropertyType.IsClass)
                {
                    var currentObject = property.GetValue(infoReader.GetOrtdp());
                    if (!(currentObject is null))
                    {
                        var currentProperties = currentObject.GetType().GetProperties();
                        foreach (var i in currentProperties)
                            if (i.GetCustomAttribute<AvailableVariableAttribute>() is AvailableVariableAttribute attra)
                                b.AppendLine($"{attra.VariableName}   {NI18n.GetLanguageElement(attra.LanguageElementName)}");
                    }
                }
                

                
            }
            if (File.Exists("vars.txt"))
            {
                MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
                provider.ComputeHash(File.ReadAllBytes("vars.txt"));
                string md5NewFile = MD5String.GetString(provider);
                provider.ComputeHash(b.ToString().ToBytes());
                string md5Target = MD5String.GetString(provider);
                if (md5NewFile != md5Target)
                {
                    IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_WRITINGHELP"));
                    File.WriteAllText("vars.txt", b.ToString());
                }
            }
            else
            {
                IO.CurrentIO.Write(NI18n.GetLanguageElement("LANG_INFO_WRITINGHELP"));
                File.WriteAllText("vars.txt", b.ToString());
            }
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = "notepad.exe", Arguments = "vars.txt" });
            return true;
        }

        public string GetHelp() => NI18n.GetLanguageElement("LANG_HELP_LISTVARS");
    }
}