using System;

namespace StarLink
{
    public abstract class ServerStarLink : StarLink
    {
        public ServerStarLink(StarProtocol protocol)
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

        public static ServerStarLink Factory(StarProtocol protocol)
        {
            switch (protocol)
            {
                case StarProtocol.TCP: return new Links.ServerStarLinkTCP();
                case StarProtocol.UDP: return new Links.ServerStarLinkUDP();
                case StarProtocol.WebSockets: return new Links.ServerStarLinkWS();
                default: throw new NotImplementedException();
            }
        }
    }
}
