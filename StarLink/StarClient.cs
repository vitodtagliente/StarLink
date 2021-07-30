using System;
using System.Net;

namespace StarLink
{
    public class StarClient
    {
        public StarClient(StarProtocol protocol)
        {
            Session = new UserSession();
            Components = new ComponentRegister<ClientComponent>();

            _link = ClientStarLink.Factory(protocol);
            _link.OnOpen = () => OnConnection.Invoke();
            _link.OnError = () => OnError.Invoke();
            _link.OnClose = () => OnDisconnection.Invoke();
            _link.OnMessage = (string data) =>
            {
                StarMessage message = MessageSerializer.Deserialize(data);
                if (message != null)
                {
                    if(message.IsCommand)
                    {
                        _commandProcessor.Process(Session, message);
                    }
                    else
                    {
                        OnMessage.Invoke(message);
                        _messageProcessor.Process(message);
                    }
                }
            };
            _messageProcessor = new ClientMessageProcessor(_link);
            _commandProcessor = new ClientCommandProcessor(_link);
        }

        public void Connect(StarEndpoint address)
        {
            _link?.Open(address);
        }

        public void Close()
        {
            _link?.Close();
        }

        public HttpStatusCode Send(StarMessage message)
        {
            return _messageProcessor.Send(message);
        }

        public HttpStatusCode Send(StarMessage request, out StarMessage response)
        {
            return _messageProcessor.Send(request, out response);
        }

        public HttpStatusCode Call<RequestType>(string commandId, RequestType request)
            where RequestType : new()
        {
            return _commandProcessor.Call(commandId, request);
        }

        public HttpStatusCode Call<RequestType, ResponseType>(string commandId, RequestType request, out ResponseType response, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            return _commandProcessor.Call(commandId, request, out response, timeout);
        }

        public StarProtocol Protocol { get { return _link.Protocol; } }
        public UserSession Session { get; private set; }
        public CommandRegister Commands { get { return _commandProcessor.Commands; } }
        public ComponentRegister<ClientComponent> Components { get; private set; }

        private ClientStarLink _link;
        private ClientMessageProcessor _messageProcessor;
        private ClientCommandProcessor _commandProcessor;

        public Action OnConnection = () => { };
        public Action OnDisconnection = () => { };
        public Action OnError = () => { };
        public Action<StarMessage> OnMessage = (StarMessage message) => { };
    }
}
