using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using InfoReaderPlugin.Command.CommandClasses;

namespace InfoReaderPlugin.Command
{
    public class CommandProcessorCollection
    {
        internal static CommandProcessorCollection GetInstance() => _instance ?? (_instance = new CommandProcessorCollection());

        private readonly Dictionary<string, ICommandProcessor> _commandProcessors = new Dictionary<string, ICommandProcessor>();
        private readonly ReadOnlyDictionary<string, ICommandProcessor> _readOnlypProcessors;
        public IReadOnlyDictionary<string, ICommandProcessor> Processors => _readOnlypProcessors;
        private static CommandProcessorCollection _instance;
        
        private CommandProcessorCollection()
        {
            _readOnlypProcessors = new ReadOnlyDictionary<string, ICommandProcessor>(_commandProcessors);
        }
        
        public ICommandProcessor this[string key]
        {
            get
            {
                if (Processors.ContainsKey(key))
                    return Processors[key];
                return null;
            }
        }
        public T GetCommandProcessor<T>(string key) where T : ICommandProcessor
        {
            var command = this[key];
            if (command is null)
                return default;
            return (T) command;
        }

        public bool AddCommandProcessor(ICommandProcessor processor,bool throwAddException = false)
        {
            if (_commandProcessors.ContainsKey(processor.MainCommand))
                return true;
            try
            {
                _commandProcessors.Add(processor.MainCommand, processor);
                return true;
            }
            catch (Exception)
            {
                if (throwAddException)
                    throw;
                return false;
            }
        }
    }
}
