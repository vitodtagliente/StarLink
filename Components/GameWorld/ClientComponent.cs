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

        // public HttpStatusCode Authenticate(AuthenticationRequest request)
        // {
        //     return Call(BaseCommand.CommandId<Commands.AuthenticationCommand>(), request);
        // }

        protected override void RegisterCommands()
        {

        }
    }
}
