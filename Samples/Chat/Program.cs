using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StarLink;

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteColorLine(ConsoleColor.Green, "1 - Server, 0 - Client");
            var line = Console.ReadLine();
            if (String.IsNullOrEmpty(line))
                line = "0";
            int choose;
            int.TryParse(line, out choose);

            if (choose == 1)
                Server();
            else Client();

            Console.Read();
        }

        static void WriteColorLine(ConsoleColor color, string line)
        {
            var fore = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = fore;
        }

        static StarClient host;

        static void Client()
        {
            host = new StarClient();
            Console.Write("Ip [localhost]: ");
            string ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip))
                ip = "localhost";
            host.Address = ip;
            Console.Write("Port [567]: ");
            string port = Console.ReadLine();
            if (string.IsNullOrEmpty(port))
                port = "567";
            host.Port = int.Parse(port);

            host.Connect();
            Console.WriteLine("Connection status: " + host.Connected.ToString());
            if (!host.Connected) return;

            Console.WriteLine("Username: ");
            string name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
                name = "Noname";

            var message = new StarMessage();
            message.Write("SET NAME " + name);

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

                if (line.ToLower().Contains("exit"))
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
                    if (line.ToLower().StartsWith("[server]"))
                        WriteColorLine(ConsoleColor.Magenta, line);
                    else WriteColorLine(ConsoleColor.Green, line);
                    if (line.ToLower().Contains("exit"))
                        host.Close();
                }
            }

            WriteColorLine(ConsoleColor.Red, DateTime.Now.ToString() + " Disconnected");
        }

        static StarServer<string> server;

        static void Server()
        {
            server = new StarServer<string>();
            Console.Write("Ip [localhost]: ");
            string ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip))
                ip = "localhost";
            server.Address = ip;
            Console.Write("Port [567]: ");
            string port = Console.ReadLine();
            if (string.IsNullOrEmpty(port))
                port = "567";
            server.Port = int.Parse(port);

            server.Connect();
            Console.WriteLine("Connection status: " + server.Connected.ToString());
            if (!server.Connected)
            {
                WriteColorLine(ConsoleColor.Cyan, "Could not start listening...");
                return;
            }
            WriteColorLine(ConsoleColor.Cyan, "Listening...");

            var AcceptThread = new Thread(ServerAcceptCallback);
            AcceptThread.Start();

            while (server.Connected)
            {
                string line = Console.ReadLine();
                var message = new StarMessage();
                message.Write("[Server] " + line);
                server.Broadcast(message);

                if (line.ToLower().Contains("exit"))
                    server.Close();
            }
        }

        static void ServerAcceptCallback()
        {
            while (server.Connected)
            {
                var client = server.Accept("Noname");
                if (client != null)
                {
                    WriteColorLine(ConsoleColor.Yellow, Time() + "New Client connected");

                    Thread serveThread = new Thread(ServeCallback);
                    serveThread.Start(client);
                }
            }
        }

        static void ServeCallback(object s)
        {
            Socket clientSocket = (Socket)s;
            WriteColorLine(ConsoleColor.Green, Time() + "ServerCallback start");
            bool clientConnected = true;

            while (server.Connected && server.IsSocketConnected(clientSocket) && clientConnected)
            {
                var message = server.ReceiveMessage(clientSocket);
                if (message != null)
                {
                    string line = message.ReadString();
                    if (line.Contains("SET NAME "))
                    {
                        server.Clients[clientSocket] = line.Replace("SET NAME ", "");
                        var response = new StarMessage();
                        response.Write("[Server] " + server.Clients[clientSocket] + " connected");
                        server.Broadcast(response, clientSocket);
                        continue;
                    }

                    string username = "[" + server.Clients[clientSocket] + "]";
                                        
                    string packet = username + " " + line;
                    Console.WriteLine(Time() + packet);
                    if (line.ToLower().Contains("exit"))
                    {
                        clientConnected = false;
                        packet = username + " disconnected";
                    }
                    var msg = new StarMessage();
                    msg.Write(packet);
                    server.Broadcast(msg, clientSocket);
                }
            }

            WriteColorLine(ConsoleColor.Red, Time() + server.Clients[clientSocket] + " disconnected");
            if (server.Connected)
            {
                var message = new StarMessage();
                message.Write(server.Clients[clientSocket] + " disconnected");
                server.Broadcast(message, clientSocket);
            }
            server.Clients.Remove(clientSocket);
        }

        static string Time()
        {
            return "[" + DateTime.Now.ToShortTimeString() + "] ";
        }
    }
}
