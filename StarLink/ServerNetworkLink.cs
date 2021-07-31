using System;

namespace StarLink
{
    abstract class ServerNetworkLink : NetworkLink
    {
        public ServerNetworkLink(StarProtocol protocol)
            : base(protocol)
        {

        }

        public override void Open(StarEndpoint address)
        {
            Listen(address.Port);
        }
        public abstract void Listen(int port);

        public Action<StarNodeId> OnNodeConnection = (NetworkNodeId) => { };
        public Action<StarNodeId> OnNodeDisconnection = (NetworkNodeId) => { };
        public Action<StarNodeId, string> OnNodeMessage = (StarNodeId nodeId, string message) => { };

        public static ServerNetworkLink Factory(StarProtocol protocol)
        {
            switch (protocol)
            {
                case StarProtocol.TCP: return new NetworkLinks.ServerLinkTCP();
                case StarProtocol.UDP: return new NetworkLinks.ServerLinkUDP();
                case StarProtocol.WebSockets: return new NetworkLinks.ServerLinkWS();
                default: throw new NotImplementedException();
            }
        }
    }
}
