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
			client.On ("connect", OnConnection);
			client.On ("disconnect", OnDisconnect);
			client.On ("message", OnMessage);
			client.Log = OnLog;

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

		static void OnConnection(StarEventData e)
		{
			ColorLine ("Connected", ConsoleColor.Green);
		}

		static void OnDisconnect(StarEventData e)
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
