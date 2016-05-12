﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace StarLink
{
	public class StarServer : StarHost
	{
		protected Thread acceptThread;
		protected Dictionary<Socket, Thread> serveThreads = new Dictionary<Socket, Thread>();

		int netMaxConnections = 100;
		public int MaxConnections
		{
			get { return netMaxConnections; }
			set
			{
				if (!Connected)
					netMaxConnections = value;
			}
		}

		public List<Socket> Clients { get; protected set; }

		public StarServer()
		{
			Clients = new List<Socket>();
		}

		public override void Start(IPEndPoint localEndPoint)
		{
			if (Connected) 
				return;
			try
			{
				Socket.Bind(localEndPoint);
				Socket.Listen(MaxConnections);

				running = true;
			}
			catch (Exception e)
			{
				Log("StarServer::Start: " + e.Message);
			}

			if (Connected) 
			{
				CallEvent ("listen", new StarEventData ());
				if (EnableThreading) {
					acceptThread = new Thread (AcceptCallback);
					acceptThread.Start ();
				}
			}
			else 
			{
				Log ("StarServer::Start: connection failed");

				CallEvent ("connection failed", new StarEventData ());
			}
		}

		void AcceptCallback()
		{
			while (Connected) 
			{
				var socket = Accept ();
				if (socket != null) {

					Thread serveThread = new Thread (ServeCallback);
					serveThreads.Add (socket, serveThread);
					serveThread.Start (socket);

				}
			}
		}

		void ServeCallback(object argument)
		{
			Socket socket = (Socket)argument;

			while (Connected && socket.IsConnected ()) 
			{
				var data = Receive (socket);
				if (data != null && 
                    //data.IsNull () == false
                    string.IsNullOrEmpty(data) == false
                    ) 
				{
					CallEvent ("message", new StarEventData (socket, data));
				}
			}

			CallEvent ("user disconnect", new StarEventData (socket));

			serveThreads.Remove (socket);
		}

		bool running = false;
		public override bool IsConnected (int pollTime = 1000)
		{
			return running;
		}

		public new void Close()
		{
			base.Close ();
			running = false;

			if (EnableThreading) {
				acceptThread.Abort ();

				foreach (var thread in serveThreads.Values)
					thread.Abort ();
			}
		}

		public Socket Accept()
		{
			if (!Connected)
				return null;

			if (Socket.Poll(0, SelectMode.SelectRead))
			{
				try
				{
					var clientSocket = Socket.Accept();
					Clients.Add(clientSocket);

					CallEvent("user connect", new StarEventData(clientSocket));

					return clientSocket;
				}
				catch (Exception e)
				{
					Log("StarServer::Accept: " + e.Message);
				}
			}
			return null;
		}

		public bool Send(Socket socket, object data){
			return base.SendData (socket, data.ToString());
		}

		public string Receive(Socket socket){
			return base.ReceiveData (socket);
		}

		public void Broadcast(object data)
		{
			foreach (var socket in Clients)
				Send(socket, data);
		}

		public void Broadcast(object data, Socket exception)
		{
			foreach (var socket in Clients)
				if(!socket.Equals(exception))
					Send(socket, data);
		}

		public void Broadcast(object data, List<Socket> exceptions)
		{
			foreach (var socket in Clients)
				if (!exceptions.Contains(socket))
					Send(socket, data);
		}
	}
}

