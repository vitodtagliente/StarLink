using System;
using System.Collections.Generic;
using System.Text;

using StarLink;

namespace Game
{
    [ComponentSettings(Id = "Game")]
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
            _server.Commands.Add(new Commands.HandShakeCommand(this, _worldManager));
            _server.Commands.Add(new Commands.SpawnCommand(this, _worldManager));
        }
    }
}
