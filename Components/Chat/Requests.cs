using System;
using StarLink;

namespace Chat
{
    [Serializable]
    class PublicMessageRequest
    {
        public string Message { get; set; }
    }

    [Serializable]
    class PrivateMessageRequest
    {
        public StarId User { get; set; }
        public string Message { get; set; }
    }
}
