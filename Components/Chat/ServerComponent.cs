using System;
using System.Collections.Generic;
using System.Text;

using StarLink;

namespace Chat
{
    [ComponentSettings(Id = "Chat")]
    public class ServerComponent : StarLink.ServerComponent
    {
        public ServerComponent(StarServer server)
            : base(server)
        {

        }

        protected override void RegisterCommands()
        {
            _server.Commands.Add(new Commands.PrivateMessageCommand(this));
            _server.Commands.Add(new Commands.PublicMessageCommand(this));
        }
    }
}
