using osuTools.Attributes;
using Sync.Command;
using System.Collections.Generic;
namespace InfoReaderPlugin.Command
{
    public class CommandParser
    {
        public List<string> Arguments { get; } = new List<string>();
        public string MainCommand { get; }
        public CommandParser(Arguments command)
        {
            MainCommand = command[0];
            for (int i = 1; i < command.Count; i++)
                Arguments.Add(command[i]);
        }
    }
    
}