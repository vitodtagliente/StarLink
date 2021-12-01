using System;
using System.Net;

using StarLink;

namespace Game.Commands
{
    [CommandSettings(Id = "Move")]
    class MoveCommand : Command<ServerComponent, MoveRequest, EmptyCommandData>
    {
        public MoveCommand(ServerComponent component, WorldManager worldManager)
            : base(component)
        {
            _worldManager = worldManager;
        }

        private WorldManager _worldManager;

        protected override HttpStatusCode ProcessRequest(UserSession userSession, MoveRequest request, out EmptyCommandData response)
        {
            response = new EmptyCommandData();

            string worldName;
            if (userSession.User.State.TryGetWorld(out worldName) == false)
            {
                return HttpStatusCode.InternalServerError;
            }

            if (_worldManager.TryGet(worldName, out World world))
            {
                StarId gameObjectId;
                if (userSession.User.State.TryGetGameObject(out gameObjectId))
                {
                    return HttpStatusCode.NotFound;
                }

                if (world.FindGameObject(gameObjectId, out GameObject gameObject))
                {
                    gameObject.Transform = request.Transform;
                    return HttpStatusCode.OK;
                }
            }
            return HttpStatusCode.NotFound;
        }
    }
}
