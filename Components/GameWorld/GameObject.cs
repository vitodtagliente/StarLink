using System;

using StarLink;

namespace GameWorld
{
    [Serializable]
    public class GameObject
    {
        public StarId Id { get; set; }
        public string Type { get; set; }
        public User UserController { get; set; }
        public Transform Transform { get; set; }

        public bool IsUserControlled { get { return UserController != null; } }
    }
}
