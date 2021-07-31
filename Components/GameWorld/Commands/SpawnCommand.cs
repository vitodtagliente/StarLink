using System;
using System.Net;

using StarLink;

namespace GameWorld.Commands
{
    [CommandSettings(Id = "Spawn")]
    class SpawnCommand : Command<SpawnRequest, SpawnResponse>
    {
        public SpawnCommand(WorldManager worldManager)
            : base()
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
