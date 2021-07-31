using System;

namespace GameWorld
{
    [Serializable]
    public class SpawnRequest
    {
        public string Type { get; set; }
        public Transform Transform { get; set; }
        public bool IsUserController { get; set; }
    }
}
