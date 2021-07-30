using System;
using System.Net;

namespace StarLink
{
    public class StarServer
    {
        public StarServer(StarProtocol protocol)
        {
            Components = new ComponentRegister<ServerComponent>();
            _sessionManager = new UserSessionManager();

            _link = ServerNetworkLink.Factory(protocol);
            _link.OnOpen = () => onListening.Invoke();
            _link.OnError = () => onError.Invoke();
            _link.OnNodeConnection = (StarNodeId nodeId) => onClientConnection.Invoke(_sessionManager.Create(nodeId));
            _link.OnNodeDisconnection = (StarNodeId nodeId) => onClientDisconnection.Invoke(_sessionManager.Create(nodeId));
            _link.OnNodeMessage = (StarNodeId nodeId, string data) =>
            {
                UserSession session = _sessionManager.Create(nodeId);

                StarMessage message = MessageSerializer.Deserialize(data);
                if (message != null)
                {
                    if(message.IsCommand)
                    {
                        _commandProcessor.Process(session, message);
                    }
                    else
                    {
                        onClientMessage.Invoke(session, message);
                        _messageProcessor.Process(message);
                    }
                }
            };
            _messageProcessor = new ServerMessageProcessor(_link);
            _commandProcessor = new ServerCommandProcessor(_link);
        }

        public void Listen(int port)
        {
            _link?.Listen(port);
        }

        public void Close()
        {
            _link?.Close();
        }

        public HttpStatusCode Send(UserSession session, StarMessage message)
        {
            return _messageProcessor.Send(session.NodeId, message);
        }

        public HttpStatusCode Send(UserSession session, StarMessage request, out StarMessage response)
        {
            return _messageProcessor.Send(session.NodeId, request, out response);
        }

        public HttpStatusCode Call<RequestType>(StarNodeId nodeId, string commandId, RequestType request)
            where RequestType : new()
        {
            return _commandProcessor.Call(nodeId, commandId, request);
        }

        public HttpStatusCode Call<RequestType, ResponseType>(StarNodeId nodeId, string commandId, RequestType request, out ResponseType response, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            return _commandProcessor.Call(nodeId, commandId, request, out response, timeout);
        }

        public StarProtocol Protocol { get { return _link.Protocol; } }
        private UserSessionManager _sessionManager;
        public CommandRegister Commands { get { return _commandProcessor.Commands; } }
        public ComponentRegister<ServerComponent> Components { get; private set; }

        private ServerNetworkLink _link;
        private ServerMessageProcessor _messageProcessor;
        private ServerCommandProcessor _commandProcessor;

        public Action<UserSession> onClientConnection = (UserSession) => { };
        public Action<UserSession> onClientDisconnection = (UserSession) => { };
        public Action<UserSession, StarMessage> onClientMessage = (UserSession userSession, StarMessage message) => { };
        public Action onError = () => { };
        public Action onListening = () => { };
    }
}
