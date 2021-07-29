using System;

namespace StarLink
{
    public abstract class StarLink
    {
        public enum LinkState
        {
            Closed,
            Connected,
            Error,
            Ready
        }

        public StarLink(StarProtocol protocol)
        {
            Protocol = protocol;
            State = LinkState.Ready;
        }

        public StarProtocol Protocol { get; private set; }
        public LinkState State { get; protected set; }

        public abstract void Open(StarEndpoint address);
        public abstract void Send(string message);
        public abstract void Send(StarNodeId nodeId, string message);
        public abstract void Close();

        public Action OnOpen = () => { };
        public Action OnClose = () => { };
        public Action OnError = () => { };
    }
}
