using System;

namespace StarLink
{
    public abstract class ClientStarLink : StarLink
    {
        public ClientStarLink(StarProtocol protocol)
            : base(protocol)
        {

        }

        public override void Send(StarNodeId nodeId, string message)
        {
            // ignore the nodeId
            Send(message);
        }

        public Action<string> OnMessage = (string message) => { };

        public static ClientStarLink Factory(StarProtocol protocol)
        {
            switch (protocol)
            {
                case StarProtocol.TCP:
                case StarProtocol.UDP: return new Links.ClientStarLinkUDP();
                case StarProtocol.WebSockets:
                default: throw new NotImplementedException();
            }
        }
    }
}
