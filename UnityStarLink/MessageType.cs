using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityStarLink
{
    public enum MessageType
    {
        SyncIdentity = 0,
        SyncTransform = 1,
        Connection = 2,
        Disconnect = 3
    }
}
