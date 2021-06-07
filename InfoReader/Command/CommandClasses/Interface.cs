namespace InfoReaderPlugin.Command.CommandClasses
{
    public interface ICommandProcessor
    {
        bool AutoCatch { get; set; }
        string MainCommand { get; }
        bool Process(InfoReader instance, CommandParser parser);
        string GetHelp();
    }
}