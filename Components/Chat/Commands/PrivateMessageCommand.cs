using System;
using System.Net;

using StarLink;

namespace Chat.Commands
{
    [CommandSettings(Id = "PrivateMessage")]
    class PrivateMessageCommand : Command<ServerComponent, PrivateMessageRequest, EmptyCommandData>
    {
        public PrivateMessageCommand(ServerComponent component)
            : base(component)
        {

        }

        protected override HttpStatusCode ProcessRequest(UserSession userSession, PrivateMessageRequest request, out EmptyCommandData response)
        {
            response = new EmptyCommandData();

            if (string.IsNullOrEmpty(request.Message))
            {
                Console.WriteLine("Chat messages cannot be empty,message {0}", request.Message);
                return HttpStatusCode.BadRequest;
            }

            // broadcast it

            return HttpStatusCode.OK;
        }
    }
}
