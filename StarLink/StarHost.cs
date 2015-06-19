using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace StarLink
{
    public class StarHost
    {
        private static readonly string DefaultAddress = "localhost";
        private static readonly int DefaultPort = 7777;

        public Socket Socket { get; protected set; }

        public bool Connected { get; protected set; }

        string netAddress;
        public string Address {
            get
            {
                return netAddress;
            }
            set
            {
                if (!Connected)
                    netAddress = value;
            }
        }

        int netPort;
        public int Port
        {
            get
            {
                return netPort;
            }
            set
            {
                if (!Connected)
                    netPort = value;
            }
        }

        ProtocolType netProtocol;
        public ProtocolType Protocol { 
            get
            {
                return netProtocol;
            }
            set
            {
                if (!Connected)
                    netProtocol = value;
            }
        }

        AddressFamily netFamily;
        public AddressFamily Family { 
            get
            {
                return netFamily;
            }
            set
            {
                if (!Connected)
                    netFamily = value;
            }
        }

        string netLog = String.Empty;
        public string Log {
            get
            {

                var temp = netLog;
                netLog = string.Empty;
                return temp;
            }
            set
            {
                netLog = value;
            }
        }

        public StarHost()
        {
            Address = DefaultAddress;
            Port = DefaultPort;
            Protocol = ProtocolType.Tcp;
            Family = AddressFamily.InterNetwork;
            Connected = false;
        }

        public void Connect(string ip, int port)
        {
            Address = ip;
            Port = port;
            Connect();
        }

        public void Connect( SocketType type = SocketType.Stream )
        {
            if (Connected)
                return;

            if (Address.ToLower().Equals("localhost"))
                Address = "127.0.0.1";
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
            Socket = new Socket(Family, type, Protocol);
            
            Start(localEndPoint);
        }

        public virtual void Start(IPEndPoint localEndPoint)
        {
            
        }

        public void Close()
        {
            if (Connected)
            {
                Connected = false;
                try
                {
                    Socket.Shutdown(SocketShutdown.Send);
                    Socket.Close();                    
                }
                catch (Exception e)
                {
                    Log = e.Message;
                }
            }
        }

        public bool IsSocketConnected(Socket socket, int pollTime = 1000)
        {
            return !((socket.Poll(pollTime, SelectMode.SelectRead)
                && (socket.Available == 0)) || !socket.Connected);
        }

        protected StarMessage ReceiveMessage(Socket socket, int pollTime = 5000)
        {
            if (!Connected)
                return null;

            if (socket.Poll(pollTime, SelectMode.SelectRead))
            {
                try
                {
                    byte[] buffer = new byte[StarMessage.BufferSize];
                    int bytesReceived = socket.Receive(buffer);
                    if (bytesReceived > 0)
                    {
                        var b = buffer.ToList<byte>();
                        b.RemoveRange(bytesReceived, b.Count - bytesReceived);
                        return new StarMessage(b.ToArray());
                    }
                }
                catch (Exception e)
                {
                    Log = e.Message;
                }
            }
            return null;
        }
                
        protected bool SendMessage(Socket socket, StarMessage message)
        {
            if (message.Empty || !Connected)
                return false;

            if (socket.Poll(0, SelectMode.SelectWrite))
            {
                try
                {
                    socket.Send(message.ToBytes());
                    return true;
                }
                catch (Exception e)
                {
                    Log = e.Message;                    
                    return false;
                }
            }
            return false;
        }
    }
}
