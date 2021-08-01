using System;
using System.Net;

using StarLink;

namespace Chat.Commands
{
    [CommandSettings(Id = "PublicMessage")]
    class ServerPublicMessageCommand : Command<ServerComponent, ServerPublicMessageRequest, EmptyCommandData>
    {
        public ServerPublicMessageCommand(ServerComponent component)
            : base(component)
        {

        }

        protected override HttpStatusCode ProcessRequest(UserSession userSession, ServerPublicMessageRequest request, out EmptyCommandData response)
        {
            response = new EmptyCommandData();

            if (string.IsNullOrEmpty(request.Message))
            {
                Console.WriteLine("Chat messages cannot be empty,message {0}", request.Message);
                return HttpStatusCode.BadRequest;
            }

            _component.Send(request.Message);

            return HttpStatusCode.OK;
        }
    }
}
