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
            _worldManager = new WorldManager();
        }

        private WorldManager _worldManager;


        protected override void RegisterCommands()
        {
            Server.Commands.Add(new Commands.HandShakeCommand(this, _worldManager));
            Server.Commands.Add(new Commands.SpawnCommand(this, _worldManager));
        }
    }
}
