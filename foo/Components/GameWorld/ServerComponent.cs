using System;
using System.Collections.Generic;
using System.Text;

using StarLink;

namespace GameWorld
{
    [ComponentSettings(Id = "GameWorld")]
    public class ServerComponent : StarLink.ServerComponent
    {
        public ServerComponent(StarServer server)
            : base(server)
        {
        }


        protected override void RegisterCommands()
        {
            // _server.Commands.Add<Commands.SpawnCommand>();
        }
    }
}
