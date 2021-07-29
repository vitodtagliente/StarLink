using System.Collections.Generic;

namespace StarLink
{
    public class CommandRegister
    {
        public CommandRegister()
        {
            _commands = new Dictionary<string, BaseCommand>();
        }

        public T Add<T>() 
            where T : BaseCommand, new()
        {
            T command = new T();
            _commands.Add(command.Id, command);
            return command;
        }

        public bool Contains(string id)
        {
            return _commands.ContainsKey(id);
        }

        public BaseCommand Get(string id)
        {
            return _commands.ContainsKey(id) ? _commands[id] : null;
        }

        public bool TryGet(string id, out BaseCommand command)
        {
            return _commands.TryGetValue(id, out command);
        }

        public void Remove(string id)
        {
            _commands.Remove(id);
        }

        private Dictionary<string, BaseCommand> _commands;
    }
}
