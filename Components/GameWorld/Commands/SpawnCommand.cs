using System;
using System.Net;

using StarLink;

namespace GameWorld.Commands
{
    [CommandSettings(Id = "Spawn")]
    class SpawnCommand : Command<ServerComponent, SpawnRequest, SpawnResponse>
    {
        public SpawnCommand(ServerComponent component, WorldManager worldManager)
            : base(component)
        {
            _worldManager = worldManager;
        }

        private WorldManager _worldManager;

        protected override HttpStatusCode ProcessRequest(UserSession userSession, SpawnRequest request, out SpawnResponse response)
        {
            response = new SpawnResponse();

            // TODO: current user world
            string worldName = "";
            if (_worldManager.TryGet(worldName, out World world))
            {
                if (world.TrySpawn(request.Type, request.Transform, out GameObject spawnedObject))
                {
                    response.GameObject = spawnedObject;
                    return HttpStatusCode.OK;
                }
            }
            return HttpStatusCode.InternalServerError;
        }
    }
}
