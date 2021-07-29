using System.Net;
using StarLink;

namespace Authentication
{
    [ComponentSettings(Id = "Authentication")]
    public class ClientComponent : StarLink.ClientComponent
    {
        public ClientComponent(StarClient client)
            : base(client)
        {
        }

        public HttpStatusCode Authenticate(AuthenticationRequest request)
        {
            return Call(BaseCommand.CommandId<Commands.AuthenticationCommand>(), request);
        }

        protected override void RegisterCommands()
        {

        }
    }
}
