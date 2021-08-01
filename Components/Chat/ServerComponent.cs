using System;
using System.Collections.Generic;
using System.Text;

using StarLink;

namespace Chat
{
    [ComponentSettings(Id = "Chat")]
    public class ServerComponent : StarLink.ServerComponent
    {
        public ServerComponent(StarServer server)
            : base(server)
        {

        }

        public void Send(string message)
        {
            // return _server.Call(BaseCommand.CommandId<Commands.ClientPublicMessageCommand>(), new ClientPublicMessageRequest()
            // {
            //     Message = message
            // });
        }

        public void Send(StarId user, string message)
        {
            // return _server.Call(BaseCommand.CommandId<Commands.ClientPrivateMessageCommand>(), new ClientPrivateMessageRequest()
            // {
            //     Message = message
            // });
        }

        protected override void RegisterCommands()
        {
            _server.Commands.Add(new Commands.ServerPrivateMessageCommand(this));
            _server.Commands.Add(new Commands.ServerPublicMessageCommand(this));
        }
    }
}
