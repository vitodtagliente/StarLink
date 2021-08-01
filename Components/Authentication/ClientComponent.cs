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
            HttpStatusCode error = Call(BaseCommand.CommandId<Commands.AuthenticationCommand>(), request, out response);
            if (error == HttpStatusCode.OK)
            {
                Client.Session.IsAuthenticated = response.Session.IsAuthenticated;
                Client.Session.Data = response.Session.Data;
                Client.Session.User = response.Session.User;
            }
            return error;
        }

        protected override void RegisterCommands()
        {

        }
    }
}
