using System;

namespace StarLink
{
    [Serializable]
    public class StarNodeId
    {
        public static readonly StarNodeId Empty = new StarNodeId
        {
            Id = string.Empty
        };

        public static StarNodeId Next()
        {
            return new StarNodeId
            {
                Id = Guid.NewGuid().ToString()
            };
        }

        public static StarNodeId Next(StarEndpoint endpoint)
        {
            return new StarNodeId
            {
                Id = string.Format("{0}:{1}", endpoint.Address, endpoint.Port)
            };
        }

        public string Id { get; set; }

        public override string ToString()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return obj is StarNodeId id &&
                   Id == id.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public bool TryGetAddress(out StarEndpoint address)
        {
            address = new StarEndpoint();
            if (Id.Contains(":"))
            {
                var pieces = Id.Split(":");
                address.Address = pieces[0];
                if (int.TryParse(pieces[1], out int port)
                    && string.IsNullOrEmpty(address.Address) == false)
                {
                    address.Port = port;
                    return true;
                }
            }
            return false;
        }
    }
}
