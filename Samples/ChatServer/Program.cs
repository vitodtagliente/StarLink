using System;
using StarLink;
using System.Net.Sockets;

namespace ChatServer
{
	class MainClass
	{
		static StarServer server;

		public static void Main (string[] args)
		{
			Console.WriteLine ("ChatServer - Console");

			server = new StarServer ();
			server.EnableThreading = true;
            server.OnConnection = OnListen;
            server.OnClientConnection = OnUserConnect;
            server.OnClientDisconnection = OnUserDisconnect;
            server.OnMessage = OnMessage;
			server.OnLog = OnLog;

			server.Connect ();

			while (server.Connected) {
				string line = Console.ReadLine ();

				if (line == "close")
					server.Close ();
			}

			Console.WriteLine ("Press a key...");
			Console.ReadKey ();
		}

		static void OnLog(string log){
			ColorLine (log, ConsoleColor.Yellow);
		}

		static void OnMessage(StarEventData e)
		{
			if (e.IsNull () == false)
				Console.WriteLine (Time() + e.data.Trim ('"'));

			server.Broadcast (e.data, e.socket);
		}

		static void OnListen()
		{
			ColorLine("Listening...", ConsoleColor.Cyan);
		}

		static void OnUserConnect(Socket client)
		{
			ColorLine("user connected...", ConsoleColor.Green);
		}

		static void OnUserDisconnect(Socket client)
		{
			ColorLine("user disconnected...", ConsoleColor.Green);
		}

		static string Time(){
			return "[" + DateTime.Now.TimeOfDay.ToString() + "] ";
		}

		static void ColorLine(string line, ConsoleColor n_color)
		{
			var color = Console.ForegroundColor;
			Console.ForegroundColor = n_color;
			Console.WriteLine (line);
			Console.ForegroundColor = color;
		}
	}
}
