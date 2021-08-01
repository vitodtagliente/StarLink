using System;
using System.Net;

using StarLink;

namespace Chat
{
    [ComponentSettings(Id = "Chat")]
    public class ClientComponent : StarLink.ClientComponent
    {
        public ClientComponent(StarClient client)
            : base(client)
        {
        }

        public HttpStatusCode Send(string message)
        {
            return Client.Call(BaseCommand.CommandId<Commands.PublicMessageCommand>(), new PublicMessageRequest()
            {
                Message = message
            });
        }

        public HttpStatusCode Send(StarId user, string message)
        {
            return Client.Call(BaseCommand.CommandId<Commands.PrivateMessageCommand>(), new PrivateMessageRequest()
            {
                User = user,
                Message = message
            });
        }

        protected override void RegisterCommands()
        {

        }

        public Action<StarId, string> OnPrivateMessage = (StarId user, string message) => { };
        public Action<string> OnPublicMessage = (string message) => { };
    }
}
