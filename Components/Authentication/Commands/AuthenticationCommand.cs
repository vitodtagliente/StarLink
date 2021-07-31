using System;
using System.Net;

using StarLink;

namespace Authentication.Commands
{
    [CommandSettings(Id = "Auth")]
    class AuthenticationCommand : Command<ServerComponent, AuthenticationRequest, AuthenticationResponse>
    {
        public AuthenticationCommand(ServerComponent component) : base(component)
        {
        }

        protected override HttpStatusCode ProcessRequest(UserSession userSession, AuthenticationRequest request, out AuthenticationResponse response)
        {
            response = new AuthenticationResponse()
            {
                Foo = "potere"
            };
            userSession.IsAuthenticated = true;
            userSession.User.State.Name = request.Username;

            Console.WriteLine("User {0} authenticated as {1}", userSession.User.Id, request.Username);

            return HttpStatusCode.OK;
        }
    }
}
