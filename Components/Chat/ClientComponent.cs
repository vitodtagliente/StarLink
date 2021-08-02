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
            return _client.Call(BaseCommand.CommandId<Commands.ServerPublicMessageCommand>(), new ServerPublicMessageRequest()
            {
                Message = message
            });
        }

        public HttpStatusCode Send(StarId user, string message)
        {
            return _client.Call(BaseCommand.CommandId<Commands.ServerPrivateMessageCommand>(), new ServerPrivateMessageRequest()
            {
                User = user,
                Message = message
            });
        }

        protected override void RegisterCommands()
        {
            _client.Commands.Add(new Commands.ClientPrivateMessageCommand(this));
            _client.Commands.Add(new Commands.ClientPublicMessageCommand(this));
        }

        public Action<StarId, string> OnPrivateMessage = (StarId user, string message) => { };
        public Action<string, string> OnPublicMessage = (string username, string message) => { };
    }
}
