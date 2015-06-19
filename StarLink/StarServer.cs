using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StarLink
{
    public class StarServer<T> : StarHost
    {
        int netMaxConnections = 100;
        public int MaxConnections
        {
            get { return netMaxConnections; }
            set
            {
                if (!Connected)
                    netMaxConnections = value;
            }
        }
        
        public Dictionary<Socket, T> Clients { get; protected set; }

        public StarServer()
        {
            Clients = new Dictionary<Socket, T>();
        }

        public override void Start(IPEndPoint localEndPoint)
        {
            if (Connected) return;
            try
            {
                Socket.Bind(localEndPoint);
                Socket.Listen(MaxConnections);
                Connected = true;
            }
            catch (Exception e)
            {
                Connected = false;
                Log = e.Message;
            }
        }
        
        public Socket Accept(T t)
        {
            if (!Connected)
                return null;

            if (Socket.Poll(0, SelectMode.SelectRead))
            {
                try
                {
                    var clientSocket = Socket.Accept();
                    Clients.Add(clientSocket, t);

                    return clientSocket;
                }
                catch (Exception e)
                {
                    Log = e.Message;
                }
            }
            return null;
        }
                         
        public new bool SendMessage(Socket socketClient, StarMessage message)
        {
            return base.SendMessage(socketClient, message);
        }

        public StarMessage ReceiveMessage(Socket socketClient)
        {
            return base.ReceiveMessage(socketClient);
        }
         
        public void Broadcast(StarMessage message)
        {
            foreach (var key in Clients.Keys)
                SendMessage(key, message);
        }

        public void Broadcast(StarMessage message, Socket exception)
        {
            foreach (var key in Clients.Keys)
                if(!key.Equals(exception))
                    SendMessage(key, message);
        }

        public void Broadcast(StarMessage message, List<Socket> exceptions)
        {
            foreach (var key in Clients.Keys)
                if (!exceptions.Contains(key))
                    SendMessage(key, message);
        }
    }
}
