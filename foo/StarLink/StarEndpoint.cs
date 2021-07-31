using System;

namespace StarLink
{
    [Serializable]
    public class StarEndpoint
    {
        public static readonly string Localhost = "127.0.0.1";
        public static readonly int DefaultPort = 17000;

        public StarEndpoint()
        {
            Address = Localhost;
            Port = DefaultPort;
        }

        public StarEndpoint(string address, int port)
        {
            Address = address;
            Port = port;
        }

        public string Address { get; set; }
        public int Port { get; set; }

        public override string ToString()
        {
            return $"{Address}:{Port}";
        }
    }
}
