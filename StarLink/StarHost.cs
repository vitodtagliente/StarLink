using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarLink
{
	public abstract class StarHost
	{
		private static readonly string ADDRESS = "localhost";
		private static readonly int PORT = 1100;
		public static int BUFFER_SIZE = 1024;

		public Socket Socket { get; protected set; }

		public bool Connected 
		{
			get {
				if (Socket == null)
					return false;
				return IsConnected (PollTime);
			}
		}

		string netAddress;
		public string Address {
			get
			{
				return netAddress;
			}
			set
			{
				if (!Connected)
					netAddress = value;
			}
		}

		int netPort;
		public int Port
		{
			get
			{
				return netPort;
			}
			set
			{
				if (!Connected)
					netPort = value;
			}
		}

		ProtocolType netProtocol;
		public ProtocolType Protocol { 
			get
			{
				return netProtocol;
			}
			set
			{
				if (!Connected)
					netProtocol = value;
			}
		}

		AddressFamily netFamily;
		public AddressFamily Family { 
			get
			{
				return netFamily;
			}
			set
			{
				if (!Connected)
					netFamily = value;
			}
		}

		public int PollTime = 1000;

		public Action<string> Log {
			get
			{
				return logHandler;
			}
			set
			{
				logHandler = value;
			}
		}

		public bool EnableThreading = true;

		private Dictionary<string, Action<StarEventData>> handlers = new Dictionary<string, Action<StarEventData>>();
		private Action<string> logHandler;

		public List<string> RegisteredEvents {
			get {				
				List<string> events = new List<string> ();
				foreach (var e in handlers.Keys)
					events.Add (e);
				return events;
			}
		}

		public StarHost()
		{
			Address = ADDRESS;
			Port = PORT;
			Protocol = ProtocolType.Tcp;
			Family = AddressFamily.InterNetwork;

			Log = LogCallback;
		}

		public void Connect(string ip, int port)
		{
			Address = ip;
			Port = port;
			Connect();
		}

		public void Connect( SocketType type = SocketType.Stream )
		{
			if (Connected)
				return;

			if (Address.ToLower().Equals("localhost"))
				Address = "127.0.0.1";
			IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
			Socket = new Socket(Family, type, Protocol);
            
            Log("StarHost::Connection(Address: " + Address + ", Port: " + Port + ")");

            Start(localEndPoint);
		}

		public void On(string ev, Action<StarEventData> callback)
		{
			if (!handlers.ContainsKey(ev)) {
				handlers [ev] = callback;
			}
		}

		void LogCallback(string log)
		{
			// todo
		}

		protected void CallEvent(string ev, StarEventData data)
		{
			if (!handlers.ContainsKey(ev)) { return; }
			try{
				handlers[ev]( data );
			} catch(Exception){

			}
		}

		public abstract void Start (IPEndPoint localEndPoint);
		public abstract bool IsConnected (int pollTime = 1000);

		public void Close()
		{
			try
			{
				Socket.Shutdown(SocketShutdown.Both);
				Socket.Close();                    
			}
			catch (Exception e)
			{
				Log("StarHost::Close: " + e.Message);
			}
		}

		protected string ReceiveData(Socket socket, int pollTime = 5000)
		{
			if (!Connected) {
				return null;
			}

			if (socket.Poll (pollTime, SelectMode.SelectRead)) {
				try {
					byte[] buffer = new byte[BUFFER_SIZE];
					int bytesReceived = socket.Receive (buffer);
					if (bytesReceived > 0) {
						var b = buffer.ToList<byte> ();
						b.RemoveRange (bytesReceived, b.Count - bytesReceived);
						string text = ASCIIEncoding.UTF8.GetString (b.ToArray ());

                        /*
						var data = StarData.Parse (text);
						if (data == null)
							return new StarData ();
						return data;
                        */
                        return text;
					}
				} catch (Exception e) {
					Log("StarHost::ReceiveData: " + e.Message);
				}
			}
			return null;
		}


		protected bool SendData(Socket socket, string data)
		{
			if (data == null || 
                //data.IsNull () ||
                string.IsNullOrEmpty(data) ||
                !Connected)
            {
				return false;
			}

			if (socket.Poll(0, SelectMode.SelectWrite))
			{
				try
				{
					socket.Send(ASCIIEncoding.UTF8.GetBytes(data.ToString()));
					return true;
				}
				catch (Exception e)
				{
					Log("StarHost::SendData: " + e.Message);                    
					return false;
				}
			}
			return false;
		}

	}
}

