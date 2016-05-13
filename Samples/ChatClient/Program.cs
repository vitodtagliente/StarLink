using System;
using StarLink;

namespace ChatClient
{
	class MainClass
	{
		static StarClient client;
		static string username;

		public static void Main (string[] args)
		{
			Console.WriteLine ("ChatClient - Console");

			Console.Write ("Username: ");
			username = Console.ReadLine ();
			Console.WriteLine ();

			client = new StarClient ();
            client.OnConnection = OnConnection;
            client.OnDisconnection = OnDisconnection;
            client.OnMessage = OnMessage;
			client.OnLog = OnLog;

			client.Connect ();

			while (client.Connected) 
			{
				string line = Console.ReadLine ();
				if (line == "close") {
					client.Close ();
					continue;
				}

				var data = (username + ": " + line);
				client.Send (data);
			}

			Console.WriteLine ("Press a key...");
			Console.ReadKey ();
		}

		static void OnLog(string log){
			ColorLine (log, ConsoleColor.Yellow);
		}

		static void OnMessage(StarEventData e)
		{
			ColorLine (e.data.ToString ().Trim ('"'), ConsoleColor.Magenta);
		}

		static void OnConnection()
		{
			ColorLine ("Connected", ConsoleColor.Green);
		}

		static void OnDisconnection()
		{
			ColorLine ("Disconnection", ConsoleColor.Green);
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
