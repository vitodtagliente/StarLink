using System.Collections;
using System.Collections.Concurrent;

namespace StarLink
{
    public class CommandRegister
    {
        public CommandRegister()
        {
            _commands = new ConcurrentDictionary<string, BaseCommand>();
        }

        public void Add(BaseCommand command)
        {
            if (_commands.TryAdd(command.Id, command))
            {

            }
        }

        public bool Contains(string id)
        {
            return _commands.ContainsKey(id);
        }

        public bool TryGet(string id, out BaseCommand command)
        {
            return _commands.TryGetValue(id, out command);
        }

        private ConcurrentDictionary<string, BaseCommand> _commands;
    }
}
