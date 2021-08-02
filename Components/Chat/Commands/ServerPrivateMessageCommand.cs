using StarLink;
using System;
using System.Net;

namespace Chat.Commands
{
    [CommandSettings(Id = "PrivateMessage", RequireResponse = false)]
    class ServerPrivateMessageCommand : Command<ServerComponent, ServerPrivateMessageRequest, EmptyCommandData>
    {
        public ServerPrivateMessageCommand(ServerComponent component)
            : base(component)
        {

        }

        protected override HttpStatusCode ProcessRequest(UserSession userSession, ServerPrivateMessageRequest request, out EmptyCommandData response)
        {
            response = new EmptyCommandData();

            if (string.IsNullOrEmpty(request.Message))
            {
                Console.WriteLine("Chat messages cannot be empty, message {0}", request.Message);
                return HttpStatusCode.BadRequest;
            }

            if (request.User == StarId.Empty)
            {
                Console.WriteLine("Chat messages cannot have invalid user, user {0}", request.User);
                return HttpStatusCode.BadRequest;
            }

            // _component.Send(request.User, request.Message);

            return HttpStatusCode.OK;
        }
    }
}
