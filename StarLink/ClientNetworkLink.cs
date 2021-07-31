using System;

namespace StarLink
{
    abstract class ClientNetworkLink : NetworkLink
    {
        public ClientNetworkLink(StarProtocol protocol)
            : base(protocol)
        {

        }

        public override void Send(StarNodeId nodeId, string message)
        {
            // ignore the nodeId
            Send(message);
        }

        public Action<string> OnMessage = (string message) => { };

        public static ClientNetworkLink Factory(StarProtocol protocol)
        {
            switch (protocol)
            {
                case StarProtocol.TCP:
                case StarProtocol.UDP: return new NetworkLinks.ClientLinkUDP();
                case StarProtocol.WebSockets:
                default: throw new NotImplementedException();
            }
        }
    }
}
