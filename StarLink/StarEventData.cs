using System.Net.Sockets;

namespace StarLink
{
	public class StarEventData
	{
		public Socket socket;

		public string data;

		public StarEventData()
		{
			socket = null;
			data = string.Empty;
		}

		public StarEventData(Socket socket)
		{
			this.socket = socket;
			this.data = string.Empty;
		}

		public StarEventData(Socket socket, string data)
		{
			this.socket = socket;
			this.data = data;
		}

		public StarEventData(string data)
		{
			this.socket = null;
			this.data = data;
		}

        public bool IsNull()
        {
            return (string.IsNullOrEmpty(data));
        }

	}
}

