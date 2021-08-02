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

        public void Send(string username, string message)
        {
            foreach (StarNodeId nodeId in _server.Clients)
            {
                _server.Call(nodeId, BaseCommand.CommandId<Commands.ClientPublicMessageCommand>(), new ClientPublicMessageRequest()
                {
                    Username = username,
                    Message = message
                });
            }
        }

        public void Send(StarNodeId nodeId, string message)
        {
            _server.Call(nodeId, BaseCommand.CommandId<Commands.ClientPrivateMessageCommand>(), new ClientPrivateMessageRequest()
            {
                Message = message
            });
        }

        protected override void RegisterCommands()
        {
            _server.Commands.Add(new Commands.ServerPrivateMessageCommand(this));
            _server.Commands.Add(new Commands.ServerPublicMessageCommand(this));
        }
    }
}
