﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StarLink.NetworkLinks
{
    class ServerLinkTCP : ServerNetworkLink
    {
        public ServerLinkTCP()
               : base(StarProtocol.TCP)
        {

        }

        public override void Send(string message)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Listen(int port)
        {
            throw new NotImplementedException();
        }

        public override void Send(StarNodeId nodeId, string message)
        {
            throw new NotImplementedException();
        }
    }
}
