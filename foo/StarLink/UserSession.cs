using System;
using System.Collections.Generic;

namespace StarLink
{
    [Serializable]
    public class UserSession
    {
        public UserSession()
        {
            NodeId = StarNodeId.Empty;
            User = new User();
            Data = new Dictionary<string, string>();
        }

        public UserSession(StarNodeId nodeId)
        {
            NodeId = nodeId;
            User = new User();
            Data = new Dictionary<string, string>();
        }

        public StarNodeId NodeId { get; private set; }
        public User User { get; set; }
        public bool IsAuthenticated { get; set; } = false;
        public Dictionary<string, string> Data { get; set; }
    }
}
