using System;
using System.Collections.Generic;
using System.Text;

using StarLink;

namespace Network
{
    [ComponentSettings(Id = "Network")]
    public class ServerComponent : StarLink.ServerComponent
    {
        public ServerComponent(StarServer server)
            : base(server)
        {
            
        }


        protected override void RegisterCommands()
        {
            _server.Commands.Add(new Commands.PingCommand(this));
        }
    }
}
