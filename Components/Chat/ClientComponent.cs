using System;
using System.Net;

using StarLink;

namespace Chat
{
    [ComponentSettings(Id = "Chat")]
    public class ClientComponent : StarLink.ClientComponent
    {
        public ClientComponent(StarClient client)
            : base(client)
        {
        }

        protected override void RegisterCommands()
        {

        }
    }
}
