using System;
using System.Net;

using StarLink;

namespace Game.Commands
{
    [CommandSettings(Id = "GameWorld.HandShake")]
    class HandShakeCommand : Command<ServerComponent, EmptyCommandData, HandShakeResponse>
    {
        public HandShakeCommand(ServerComponent component, WorldManager worldManager)
            : base(component)
        {
            _worldManager = worldManager;
        }

        private WorldManager _worldManager;

        protected override HttpStatusCode ProcessRequest(UserSession userSession, EmptyCommandData request, out HandShakeResponse response)
        {
            response = new HandShakeResponse();


            
            return HttpStatusCode.InternalServerError;
        }
    }
}
