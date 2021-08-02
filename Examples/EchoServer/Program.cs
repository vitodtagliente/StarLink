using StarLink;
using System;

namespace EchoServer
{
    class Program
    {
        static readonly int DefaultPort = 7000;

        static void Main()
        {
            StarServer server = new StarServer(StarProtocol.WebSockets);
            server.OnClientMessage = (UserSession session, StarMessage message) =>
            {
                // echo server
                Console.WriteLine("The client {0} sent the message: {1}", session.User.Id, message.Body);
                StarMessage response = StarMessage.Create();
                response.Body = message.Body;
                server.Send(session, response);
            };
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
