using System;
using System.Net;

using StarLink;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            StarServer server = new StarServer(StarProtocol.UDP);
            server.OnClientMessage = (UserSession session, StarMessage message) =>
            {
                // echo server
                Console.WriteLine("The client {0} sent the message: {1}", session.User.Id, message.Body);
                StarMessage response = StarMessage.Create();
                response.Body = string.Format("Hello Client {0}", session.User.Id);
                server.Send(session, response);
            };
            server.Components.Add(new Authentication.ServerComponent(server));
            server.Components.Add(new Chat.ServerComponent(server));
            server.Components.Add(new GameWorld.ServerComponent(server));
            server.Components.Add(new Network.ServerComponent(server));

            StarClient client = new StarClient(StarProtocol.UDP)
            {
                OnMessage = (StarMessage message) =>
                {
                    Console.WriteLine("The server sent the message: {0}", message.Body);
                }
            };
            client.Components.Add(new Authentication.ClientComponent(client));
            client.Components.Add(new Chat.ClientComponent(client));
            client.Components.Add(new GameWorld.ClientComponent(client));
            client.Components.Add(new Network.ClientComponent(client));

            if (server != null && client != null)
            {
                server.Listen(7000);

                client.Connect(new StarEndpoint(StarEndpoint.Localhost, 7000));

                // welcome message
                StarMessage message = StarMessage.Create();
                message.Body = "Hello Server";
                client.Send(message);

                HttpStatusCode error = HttpStatusCode.OK;
                if (client.Components.TryGet(out Authentication.ClientComponent authComponent))
                {
                    error = authComponent.Authenticate(new Authentication.AuthenticationRequest()
                    {
                        Username = "Vito"
                    });
                }

                if (client.Components.TryGet(out Network.ClientComponent netComponent))
                {
                    netComponent.Ping(out int ping);
                    Console.WriteLine("Ping {0}ms", ping);
                }

                if (client.Components.TryGet(out Chat.ClientComponent chatComponent))
                {
                    chatComponent.OnPublicMessage = (string username, string message) =>
                    {
                        Console.WriteLine("[Chat][Public][{0}] {1}: {2}", DateTime.Now.ToShortTimeString(), username, message);
                    };

                    chatComponent.Send("Ciao");
                    chatComponent.Send("Come va?");
                }

                Console.ReadKey(true);
                client.Close();
                server.Close();
            }
        }
    }
}
