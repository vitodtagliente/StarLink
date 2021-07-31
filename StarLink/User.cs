using System;
using System.Collections.Generic;

namespace StarLink
{
    [Serializable]
    public class UserState
    {
        public UserState()
        {
            Name = string.Empty;
            Data = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }

    public class User
    {
        public User()
        {
            Id = StarId.Next();
            State = new UserState();
        }

        public StarId Id { get; private set; }
        public UserState State { get; set; }
    }
}
