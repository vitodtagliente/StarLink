using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game
{
    public class WorldManager
    {
        public WorldManager()
        {
            _worlds = new ConcurrentDictionary<string, World>();
        }

        public bool TryGet(string name, out World world)
        {
            if (!_worlds.ContainsKey(name))
            {
                _worlds.TryAdd(name, new World(name));
            }

            return _worlds.TryGetValue(name, out world);
        }

        public ICollection<World> Worlds { get { return _worlds.Values; } }

        private ConcurrentDictionary<string, World> _worlds;
    }
}
