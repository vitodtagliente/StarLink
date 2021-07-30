using System;
using System.Net;

using StarLink;

namespace GameWorld.Commands
{
    [CommandSettings(Id = "Spawn")]
    class SpawnCommand : Command<SpawnRequest, EmptyCommandResponse>
    {
        protected override HttpStatusCode ProcessRequest(UserSession userSession, SpawnRequest request, out EmptyCommandResponse response)
        {
            response = new EmptyCommandResponse();



            return HttpStatusCode.OK;
        }
    }
}
