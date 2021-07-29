using System;

namespace StarLink
{
    [Serializable]
    public class StarId
    {
        public static readonly StarId Empty = new StarId
        {
            Id = string.Empty
        };

        public static StarId Next()
        {
            return new StarId
            {
                Id = Guid.NewGuid().ToString()
            };
        }

        public string Id { get; set; }

        public override string ToString()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return obj is StarId id &&
                   Id == id.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
