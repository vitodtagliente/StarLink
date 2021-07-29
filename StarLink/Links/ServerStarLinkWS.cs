using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StarLink.Links
{
    class ServerStarLinkWS : ServerStarLink
    {
        public ServerStarLinkWS()
            : base(StarProtocol.WebSockets)
        {
            clients = new Dictionary<StarNodeId, WebSocket>();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        public override void Send(string message)
        {
            if (State == LinkState.Connected)
            {

            }
        }

        public override void Close()
        {
            if (httpListener?.IsListening ?? false)
            {
                Console.WriteLine("Server is stopping.");
                State = LinkState.Closed;

                tokenSource.Cancel();
                httpListener.Stop();
                httpListener.Close();
                tokenSource.Dispose();
            }
        }

        public override void Listen(int port)
        {
            if (State != LinkState.Ready)
            {
                return;
            }

            httpListener = new HttpListener();
            httpListener.Prefixes.Add($"http://localhost:{port}/");
            httpListener.Start();

            State = LinkState.Connected;
            Console.WriteLine("Listening...");

            if (httpListener.IsListening)
            {
                State = LinkState.Connected;
                Task.Run(() => AcceptConnection().ConfigureAwait(false));
            }
            else
            {
                State = LinkState.Error;
                Console.WriteLine("The server failed to start");
            }
        }

        private async Task AcceptConnection()
        {
            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext wsContext = null;
                    try
                    {
                        wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        StarNodeId nodeId = new StarNodeId();
                        Console.WriteLine($"Socket {nodeId} connected");
                        _ = Task.Run(() => ProcessWebSocket(wsContext, nodeId)).ConfigureAwait(false);
                    }
                    catch
                    {
                        // server error if upgrade from HTTP to WebSocket fails
                        context.Response.StatusCode = 500;
                        context.Response.Close();
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task ProcessWebSocket(HttpListenerWebSocketContext context, StarNodeId nodeId)
        {
            var socket = context.WebSocket;
            try
            {
                clients.Add(nodeId, socket);
                OnNodeConnection(nodeId);

                byte[] buffer = new byte[4096];
                while (socket.State == WebSocketState.Open && !token.IsCancellationRequested)
                {
                    WebSocketReceiveResult receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine($"Socket {nodeId}: Closing websocket.");
                        OnNodeDisconnection(nodeId);
                        clients.Remove(nodeId);

                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", token);
                    }
                    else
                    {
                        string message = ASCIIEncoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                        if (string.IsNullOrEmpty(message) == false)
                        {
                            OnNodeMessage.Invoke(nodeId, message);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // normal upon task/token cancellation, disregard
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Socket {nodeId}: Exception {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"Socket {nodeId}: Inner Exception {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
            }
            finally
            {
                socket?.Dispose();
            }
        }

        public override async void Send(StarNodeId nodeId, string message)
        {
            if (httpListener?.IsListening ?? false)
            {
                WebSocket socket = clients[nodeId];
                if (socket != null)
                {
                    byte[] buffer = ASCIIEncoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Binary, true, token).ConfigureAwait(false);
                }
            }
        }

        private HttpListener httpListener;
        private Dictionary<StarNodeId, WebSocket> clients;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;
    }
}
