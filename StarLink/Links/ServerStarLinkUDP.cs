using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StarLink.Links
{
    static class IPEndPointExtension
    {
        public static StarEndpoint ToStarEndpoint(this IPEndPoint endpoint)
        {
            return new StarEndpoint(endpoint.Address.ToString(), endpoint.Port);
        }
    }

    class ServerStarLinkUDP : ServerStarLink
    {
        public ServerStarLinkUDP()
            : base(StarProtocol.UDP)
        {
            TimedDictionarySettings settings = new TimedDictionarySettings
            {
                TickTime = 60000,
                RefreshTTLOnRead = true,
                TTL = 60000
            };
            _clients = new TimedDictionary<StarNodeId, IPEndPoint>(settings);
            _translationMap = new Dictionary<IPEndPoint, StarNodeId>();
            _clients.onExpire = (StarNodeId nodeId, IPEndPoint endpoint) =>
            {
                OnNodeDisconnection(nodeId);
                _translationMap.Remove(endpoint);
            };
        }

        public override void Send(string message)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            try
            {
                OnClose.Invoke();
                _listener.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception {e.GetType().Name}: {e.Message}");
                if (e.InnerException != null) Console.WriteLine($"Inner Exception {e.InnerException.GetType().Name}: {e.InnerException.Message}");
            }
        }

        public override void Listen(int port)
        {
            if (State == LinkState.Connected)
                return;

            try
            {
                _listener = new UdpClient(port, AddressFamily.InterNetwork);
                Task.Run(() => ReceiveMessages().ConfigureAwait(false));

                State = LinkState.Connected;
                OnOpen.Invoke();
                Console.WriteLine("Listening...");
            }
            catch (Exception e)
            {
                State = LinkState.Error;
                OnError.Invoke();
                Console.WriteLine("The server failed to start");
                Console.WriteLine($"Exception {e.GetType().Name}: {e.Message}");
                if (e.InnerException != null) Console.WriteLine($"Inner Exception {e.InnerException.GetType().Name}: {e.InnerException.Message}");
            }
        }
        private async Task ReceiveMessages()
        {
            while (State == LinkState.Connected)
            {
                try
                {
                    UdpReceiveResult result = await _listener.ReceiveAsync();
                    string message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length);
                    IPEndPoint endpoint = result.RemoteEndPoint;

                    StarNodeId nodeId = null;
                    if (_translationMap.ContainsKey(endpoint) == false)
                    {
                        nodeId = StarNodeId.Next();
                        OnNodeConnection.Invoke(nodeId);
                        _clients.Add(nodeId, endpoint);
                        _translationMap.Add(endpoint, nodeId);
                    }
                    else
                    {
                        nodeId = _translationMap[endpoint];
                    }

                    if (string.IsNullOrEmpty(message) == false)
                    {
                        OnNodeMessage(nodeId, message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception {e.GetType().Name}: {e.Message}");
                }
            }
        }

        public override void Send(StarNodeId nodeId, string message)
        {
            if (_clients.ContainsKey(nodeId))
            {
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    _listener.SendAsync(bytes, bytes.Length, _clients[nodeId]);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception {e.GetType().Name}: {e.Message}");
                    if (e.InnerException != null) Console.WriteLine($"Inner Exception {e.InnerException.GetType().Name}: {e.InnerException.Message}");
                }
            }
        }

        private UdpClient _listener;
        private TimedDictionary<StarNodeId, IPEndPoint> _clients;
        private Dictionary<IPEndPoint, StarNodeId> _translationMap;
    }
}
