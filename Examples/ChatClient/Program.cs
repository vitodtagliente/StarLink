using StarLink;
using System;
using System.Net;

namespace ChatClient
{
    class Program
    {
        static readonly int DefaultPort = 7000;
        static readonly StarEndpoint DefaultEdnpoint = new StarEndpoint(StarEndpoint.Localhost, DefaultPort);

        static void Main()
        {
            StarClient client = new StarClient(StarProtocol.UDP);
            client.Components.Add(new Authentication.ClientComponent(client));
            client.Components.Add(new Chat.ClientComponent(client));
            client.Components.Add(new Network.ClientComponent(client));
            client.OnError = () =>
            {
                Console.WriteLine("Failed to connet to the endpoint {0}", DefaultEdnpoint);
            };

            client.Connect(DefaultEdnpoint);

            if (Authenticate(client))
            {
                if (client.Components.TryGet(out Chat.ClientComponent chatComponent))
                {
                    chatComponent.OnPublicMessage = (string username, string message) =>
                    {
                        Console.WriteLine("[Chat][Public][{0}] {1}: {2}", DateTime.Now.ToShortTimeString(), username, message);
                    };

                    Console.WriteLine("Enter [Exit] to terminate the application...");
                    string text = string.Empty;
                    do
                    {
                        text = Console.ReadLine();
                        if (string.IsNullOrEmpty(text) == false)
                        {
                            chatComponent.Send(text);
                        }
                    }
                    while (text != "Exit");
                }
            }            

            Console.WriteLine("Press a key to terminate the application...");
            Console.ReadKey(true);
            client.Close();
        }

        private static bool Authenticate(StarClient client)
        {
            Console.Write("Enter the username: ");
            string username = Console.ReadLine();

            if (client.Components.TryGet(out Authentication.ClientComponent authComponent))
            {
                HttpStatusCode error = authComponent.Authenticate(new Authentication.AuthenticationRequest()
                {
                    Username = username
                });

                if (error == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            Console.WriteLine("Failed to authenticate");
            return false;
        }
    }
}
