using System;
using System.Net;

using StarLink;

namespace Chat.Commands
{
    [CommandSettings(Id = "PrivateMessage", RequireAuthentication = true)]
    class ClientPrivateMessageCommand : Command<ClientComponent, ClientPrivateMessageRequest, EmptyCommandData>
    {
        public ClientPrivateMessageCommand(ClientComponent component)
            : base(component)
        {

        }

        protected override HttpStatusCode ProcessRequest(UserSession userSession, ClientPrivateMessageRequest request, out EmptyCommandData response)
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



            return HttpStatusCode.OK;
        }
    }
}
