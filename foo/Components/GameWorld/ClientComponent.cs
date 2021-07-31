using System;
using System.Net;

using StarLink;

namespace GameWorld
{
    [ComponentSettings(Id = "GameWorld")]
    public class ClientComponent : StarLink.ClientComponent
    {
        public ClientComponent(StarClient client)
            : base(client)
        {
        }

        public HttpStatusCode Spawn(SpawnRequest request)
        {
            return Call(BaseCommand.CommandId<Commands.SpawnCommand>(), request);
        }

        protected override void RegisterCommands()
        {

        }
    }
}
