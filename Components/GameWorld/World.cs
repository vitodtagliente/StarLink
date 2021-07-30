using System;
using System.Collections.Generic;
using System.Text;

namespace GameWorld
{
    [Serializable]
    public class World
    {
        public string Name { get; set; }

        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();
    }
}
