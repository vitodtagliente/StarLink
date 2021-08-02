using StarLink;
using System.Net;

namespace Chat
{
    [ComponentSettings(Id = "Chat")]
    public class ServerComponent : StarLink.ServerComponent
    {
        public ServerComponent(StarServer server)
            : base(server)
        {

        }

        public HttpStatusCode Send(string username, string message)
        {
            foreach (StarNodeId nodeId in _server.Clients)
            {
                _server.Call(nodeId, BaseCommand.CommandId<Commands.ClientPublicMessageCommand>(), new ClientPublicMessageRequest()
                {
                    Username = username,
                    Message = message
                });
            }
            return HttpStatusCode.OK;
        }

        public HttpStatusCode Send(StarNodeId nodeId, string message)
        {
            return _server.Call(nodeId, BaseCommand.CommandId<Commands.ClientPrivateMessageCommand>(), new ClientPrivateMessageRequest()
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
