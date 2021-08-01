using System;
using StarLink;

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
        public string Message { get; set; }
    }

    [Serializable]
    class ClientPrivateMessageRequest
    {
        public StarId User { get; set; }
        public string Message { get; set; }
    }
}
