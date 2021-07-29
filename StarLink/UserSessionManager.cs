using System;
using System.Collections.Generic;
using System.Text;

namespace StarLink
{
    class UserSessionManager : Dictionary<StarNodeId, UserSession>
    {
        public UserSessionManager()
            : base()
        {

        }

        public UserSession Create(StarNodeId nodeId)
        {
            if (ContainsKey(nodeId))
            {
                return this[nodeId];
            }

            UserSession session = new UserSession(nodeId);
            Add(nodeId, session);
            return session;
        }

        public StarNodeId GetNodeId(UserSession session)
        {
            foreach (var pair in this)
            {
                if (pair.Value == session)
                    return pair.Key;
            }
            return null;
        }
    }
}
