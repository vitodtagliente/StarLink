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

        public HttpStatusCode HandShake(out HandShakeResponse response)
        {
            return Call(BaseCommand.CommandId<Commands.HandShakeCommand>(), new EmptyCommandData(), out response);
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
