using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StarLink.Links
{
    class ClientStarLinkUDP : ClientStarLink
    {
        public ClientStarLinkUDP()
            : base(StarProtocol.UDP)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public override void Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception {e.GetType().Name}: {e.Message}");
                if (e.InnerException != null) Console.WriteLine($"Inner Exception {e.InnerException.GetType().Name}: {e.InnerException.Message}");
            }
        }

        public override void Open(StarEndpoint address)
        {
            if (State == LinkState.Connected)
                return;

            try
            {
                _socket.Connect(new IPEndPoint(IPAddress.Parse(address.Address), address.Port));
                OnOpen.Invoke();

                Task.Run(() => ReceiveMassage().ConfigureAwait(false));

                State = LinkState.Connected;
                Console.WriteLine($"Connected to {address}");
            }
            catch (Exception e)
            {
                State = LinkState.Error;
                Console.WriteLine($"Failed to connect to {address}");
                Console.WriteLine($"Exception {e.GetType().Name}: {e.Message}");
                if (e.InnerException != null) Console.WriteLine($"Inner Exception {e.InnerException.GetType().Name}: {e.InnerException.Message}");

            }
        }

        private async Task ReceiveMassage()
        {
            while (State == LinkState.Connected && IsConnected(_socket))
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesReceived = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesReceived > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                        if (string.IsNullOrEmpty(message) == false)
                        {
                            OnMessage.Invoke(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception {e.GetType().Name}: {e.Message}");
                    if (e.InnerException != null) Console.WriteLine($"Inner Exception {e.InnerException.GetType().Name}: {e.InnerException.Message}");
                }
            }
        }

        private static bool IsConnected(Socket socket, int pollTime = 1000)
        {
            try
            {
                bool part1 = socket.Poll(pollTime, SelectMode.SelectRead);
                bool part2 = (socket.Available == 0);
                if (part1 & part2)
                {
                    // connection is closed 
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void Send(string message)
        {
            if (_socket.Poll(0, SelectMode.SelectWrite))
            {
                try
                {
                    _socket.SendAsync(Encoding.UTF8.GetBytes(message), SocketFlags.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception {e.GetType().Name}: {e.Message}");
                    if (e.InnerException != null) Console.WriteLine($"Inner Exception {e.InnerException.GetType().Name}: {e.InnerException.Message}");
                }
            }
        }

        private Socket _socket;
    }
}
