﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StarLink
{
	public class StarClient : StarHost
	{
		protected Thread receiveThread;
                
		public override void Start(IPEndPoint endPoint)
		{
			if (Connected)
				return;
			try
			{
				Socket.Connect(endPoint);
			}
			catch (Exception e)
			{
				Log("StarClient::Start: " + e.Message);
			}

			if (Connected) {
				Log ("StarClient::Start: connected");

                if (OnConnection != null)
                    OnConnection.Invoke();

				if (EnableThreading) {
					receiveThread = new Thread (ReceiveCallback);
					receiveThread.Start ();
				}
			} 
			else 
			{
				Log ("StarClient::Start: connection failed");

                if (OnConnectionFailed != null)
                    OnConnectionFailed.Invoke();
			}
		}

		bool lastConnectionState = false;
		public override bool IsConnected (int pollTime = 1000)
		{
			var state = false;
			try {
				state = (!((Socket.Poll (pollTime, SelectMode.SelectRead)
					&& (Socket.Available == 0)) || !Socket.Connected));
			}
			catch(Exception){}
			if (lastConnectionState != state) {
				if (lastConnectionState) {
					Close ();

					receiveThread.Abort ();

                    if (OnDisconnection != null)
                        OnDisconnection.Invoke();
                }
			}
			lastConnectionState = state;
			return state;
		}

		void ReceiveCallback()
		{
			Log ("StarClient::ReceiveCallback: running...");
			while (Connected) 
			{
				var data = Receive ();
				if (data != null && string.IsNullOrEmpty(data) == false)
                {
					Log ("StarClient::Receive: new message");

                    if (OnMessage != null)
                        OnMessage.Invoke(new StarEventData(data));
                }
			}
		}

		public bool Send(string data)
		{
			return base.SendData(Socket, data);
		}

        public bool Send(object data)
        {
            return base.SendData(Socket, data.ToString());
        }

		public string Receive()
		{
			return base.ReceiveData(Socket);
		}

	}
}

