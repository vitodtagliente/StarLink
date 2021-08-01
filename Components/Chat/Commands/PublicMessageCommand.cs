using System;
using System.Net;

using StarLink;

namespace Chat.Commands
{
    [CommandSettings(Id = "PublicMessage")]
    class PublicMessageCommand : Command<ServerComponent, PublicMessageRequest, EmptyCommandData>
    {
        public PublicMessageCommand(ServerComponent component)
            : base(component)
        {

        }

        protected override HttpStatusCode ProcessRequest(UserSession userSession, PublicMessageRequest request, out EmptyCommandData response)
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
