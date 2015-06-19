using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StarLink;
using System.Net;
using System.Net.Sockets;

namespace LibraryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1 - Server, 0 - Client");
            var line = Console.ReadLine();
            if (String.IsNullOrEmpty(line))
                line = "0";
            int choose = int.Parse(line);

            if (choose == 1)
                Server();
            else Client();

            Console.Read();
        }

        static StarClient host;

        static void Client()
        {
            host = new StarClient();
            host.Address = "localhost";
            host.Port = 567;

            host.Connect();
            Console.WriteLine("Connection status: " + host.Connected.ToString());
            if (!host.Connected) return;

            var message = new StarMessage();
            message.Write("Welcome, im a client");

            var result = host.SendMessage(message);
            if (result == false)
            {
                Console.WriteLine("Error Login Message: " + host.Log);
            }

            var ListenThread = new Thread(ClientListenCallback);
            ListenThread.Start();

            while (host.Connected)
            {
                string line = Console.ReadLine();
                
                message = new StarMessage();
                message.Write(line);
                if (!host.SendMessage(message))
                    WriteColorLine(ConsoleColor.Red, "Error Sending Message: " + message.ToString());

                if (line.Equals("exit"))
                    host.Close();
            }
        }

        static void ClientListenCallback()
        {
            while (host.Connected)
            {
                var msg = host.ReceiveMessage();
                if (msg != null && !msg.Empty)
                {
                    string line = msg.ReadString();
                    Console.WriteLine("Packet: " + line);
                    if (line.ToLower().Equals("exit"))
                        host.Close();
                }
            }

            WriteColorLine(ConsoleColor.Red, DateTime.Now.ToString() + " Disconnected");
        }

        static void WriteColorLine(ConsoleColor color, string line)
        {
            var fore = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = fore;
        }

        static StarServer<string> server;

        static void Server()
        {
            server = new StarServer<string>();
            server.Address = "localhost";
            server.Port = 567;

            server.Connect();
            Console.WriteLine("Connection status: " + server.Connected.ToString());
            if (!server.Connected) return;
            WriteColorLine(ConsoleColor.Cyan, "Listening...");

            var AcceptThread = new Thread(ServerAcceptCallback);
            AcceptThread.Start();

            while (server.Connected)
            {
                string line = Console.ReadLine();
                var message = new StarMessage();
                message.Write(line);
                server.Broadcast(message);

                if (line.ToLower().Equals("exit"))
                    server.Close();
            }
        }

        static void ServerAcceptCallback()
        {
            while (server.Connected)
            {
                var client = server.Accept("Client");
                if (client != null)
                {
                    WriteColorLine(ConsoleColor.Yellow, DateTime.Now.ToString() +  " New Client connected");

                    Thread serveThread = new Thread(ServeCallback);
                    serveThread.Start(client);
                }
            }
        }

        static void ServeCallback(object s)
        {
            Socket clientSocket = (Socket)s;
            WriteColorLine(ConsoleColor.Green, DateTime.Now.ToString() + " ServerCallback");
            bool clientConnected = true;

            while (server.Connected && server.IsSocketConnected(clientSocket) && clientConnected)
            {
                var message = server.ReceiveMessage(clientSocket);
                if (message != null)
                {
                    string line = message.ReadString();
                    Console.WriteLine(DateTime.Now.ToString() + " " + line);

                    if (line.ToLower().Equals("exit"))
                        clientConnected = false;
                }
            }

            WriteColorLine(ConsoleColor.Red, DateTime.Now.ToString() + " Client disconnected");
        }
    }
}
