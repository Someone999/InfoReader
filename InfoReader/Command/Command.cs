using System;
using InfoReaderPlugin.Command;
using InfoReaderPlugin.Command.CommandClasses;
using InfoReaderPlugin.Exceptions;
using InfoReaderPlugin.I18n;

namespace InfoReaderPlugin
{
    using Sync.Command;
    using Sync.Plugins;
    using Sync.Tools;

    public partial class InfoReader
    {
        public bool GetInfo(Arguments args)
        {
            CommandParser parser = new Command.CommandParser(args);
            ICommandProcessor processor = null;
            if (CommandProcessors.Processors.ContainsKey(parser.MainCommand))
                processor = CommandProcessors[parser.MainCommand];
            try
            {
                if (processor != null)
                {
                    processor.Process(this, parser);
                }
                else
                {
                    IO.CurrentIO.Write(CommandProcessors["help"].GetHelp());
                    return true;
                }
               
            }
            catch(Exception ex)
            {

                if(processor is ICommandProcessor p)
                    if (!p.AutoCatch)
                        throw new CommandProcessorException(NI18n.GetLanguageElement("LANG_ERR_PROCESSOREXCEPTION"), p,
                            ex);
                    else
                    {
                       bool handled = p.OnUnhandledException(this, ex);
                       if (!handled)
                       {
                           string notification = string.Format(NI18n.GetLanguageElement("LANG_ERR_UNHANDLEDEXCEPTION"),
                               p.MainCommand,ex);
                           IO.CurrentIO.WriteColor(notification,ConsoleColor.Red);
                       }
                    }
            }
            EventBus.RaiseEvent(new InfoReaderPluginEvent("getinfo"));
            return true;
        }

        void CommandInit(PluginEvents.InitCommandEvent e)
        {
            e.Commands.Dispatch.bind("getinfo", GetInfo, NI18n.GetLanguageElement("LANG_HELP_INFO"));//普通的获取信息，在Sync中输入getinfo help查看详细信息
        }
    }
}