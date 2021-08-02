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
            HttpStatusCode error = Call(BaseCommand.CommandId<Commands.AuthenticationCommand>(), request, out AuthenticationResponse response);
            if (error == HttpStatusCode.OK)
            {
                _client.Session.IsAuthenticated = response.Session.IsAuthenticated;
                _client.Session.Data = response.Session.Data;
                _client.Session.User = response.Session.User;
            }
            return error;
        }

        protected override void RegisterCommands()
        {

        }
    }
}
