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

        public HttpStatusCode Authenticate(AuthenticationRequest request, out AuthenticationResponse response)
        {
            return Call(BaseCommand.CommandId<Commands.AuthenticationCommand>(), request, out response);
        }

        protected override void RegisterCommands()
        {

        }
    }
}
