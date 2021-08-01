using System;
using System.Collections.Generic;
using System.Text;

namespace Chat
{
    [Serializable]
    class WriteRequest
    {
        public string Message { get; set; }
    }
}
