using StarLink;
using System;

namespace Chat
{
    [Serializable]
    class ServerPublicMessageRequest
    {
        public string Message { get; set; }
    }

    [Serializable]
    class ServerPrivateMessageRequest
    {
        public StarId User { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    class ClientPublicMessageRequest
    {
        public string Username { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    class ClientPrivateMessageRequest
    {
        public StarId User { get; set; }
        public string Message { get; set; }
    }
}
