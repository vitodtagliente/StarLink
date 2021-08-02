using StarLink;
using System;

namespace ChatServer
{
    class Program
    {
        static readonly int DefaultPort = 7000;

        static void Main()
        {
            StarServer server = new StarServer(StarProtocol.UDP);
            server.Components.Add(new Authentication.ServerComponent(server));
            server.Components.Add(new Chat.ServerComponent(server));
            server.Components.Add(new Network.ServerComponent(server));
            server.OnListening = () =>
            {
                Console.WriteLine("Server listening at port {0}...", DefaultPort);
            };
            server.Listen(DefaultPort);

            Console.WriteLine("Press a key to terminate the application...");
            Console.ReadKey(true);
            server.Close();
        }
    }
}
