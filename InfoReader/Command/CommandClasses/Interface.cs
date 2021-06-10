using System;

namespace InfoReaderPlugin.Command.CommandClasses
{
    public interface ICommandProcessor
    {
        bool OnUnhandledException(InfoReader instance,Exception exception);
        bool AutoCatch { get; set; }
        string MainCommand { get; }
        bool Process(InfoReader instance, CommandParser parser);
        string GetHelp();
    }
}