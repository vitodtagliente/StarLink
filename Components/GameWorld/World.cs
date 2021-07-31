using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

using StarLink;

namespace GameWorld
{
    [Serializable]
    public class World
    {
        public World()
        {
            _objects = new ConcurrentDictionary<StarId, GameObject>();
        }

        public World(string name)
        {
            Name = name;
            _objects = new ConcurrentDictionary<StarId, GameObject>();
        }

        public string Name { get; set; }

        private ConcurrentDictionary<StarId, GameObject> _objects;
        public ICollection<GameObject> GameObjects { get { return _objects.Values; } }

        public bool TrySpawn(string type, Transform transform, out GameObject gameObject)
        {
            gameObject = new GameObject
            {
                Id = StarId.Next(),
                Type = type,
                Transform = transform
            };
            return _objects.TryAdd(gameObject.Id, gameObject);
        }
    }
}
