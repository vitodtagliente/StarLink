using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace StarLink
{
    public class StarClient : StarHost
    {
        public override void Start(IPEndPoint endPoint)
        {
            if (Connected) return;
            try
            {
                Socket.Connect(endPoint);
                Connected = true;
            }
            catch (Exception e)
            {
                Connected = false;
                Log = e.Message;
            }
        }

        public bool SendMessage(StarMessage message)
        {
            return base.SendMessage(Socket, message);
        }

        public StarMessage ReceiveMessage()
        {
            if (!IsSocketConnected(Socket))
            {
                Close();
                return null;
            }
            return base.ReceiveMessage(Socket);
        }
    }
}
