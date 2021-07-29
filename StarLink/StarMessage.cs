using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace StarLink
{
    public static class HeaderTypeExtensions
    {
        public static string GetDescription(this MessageHeader.HeaderType header)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])header
               .GetType()
               .GetField(header.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    [Serializable]
    public class MessageHeader
    {
        public static readonly double Version = 1.0;

        public enum HeaderType
        {
            [Description("version")] Version,
            [Description("command")] Command,
            [Description("status_code")] StatusCode,
        }

        public bool Contains(string header) { return Fields.ContainsKey(header); }
        public bool Contains(HeaderType header) { return Contains(header.GetDescription()); }
        public string Get(string header) { return Fields.ContainsKey(header) ? Fields[header] : string.Empty; }
        public string Get(HeaderType header) { return Get(header.GetDescription()); }
        public void Set(string header, string value) { Fields[header] = value; }
        public void Set(HeaderType header, string value) { Set(header.GetDescription(), value); }
        public void Set<T>(string header, T value) { Set(header, value.ToString()); }
        public void Set<T>(HeaderType header, T value) { Set(header.GetDescription(), value.ToString()); }

        public StarId Id { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }

    [Serializable]
    public class StarMessage
    {
        public static StarMessage Create(StarId id = null)
        {
            return new StarMessage
            {
                Header = new MessageHeader
                {
                    Id = (id != null && id != StarId.Empty) ? id : StarId.Next(),
                    Fields = new Dictionary<string, string>()
                    {
                        { MessageHeader.HeaderType.Version.GetDescription(), MessageHeader.Version.ToString() },
                        { MessageHeader.HeaderType.StatusCode.GetDescription(), 200.ToString() /* OK */ }
                    }
                },
                Body = string.Empty
            };
        }

        public MessageHeader Header { get; set; }
        public string Body { get; set; }
    }

    public static class MessageSerializer
    {
        public static string Serialize(StarMessage message)
        {
            return StarSerializer.Serialize<StarMessage>(message);
        }

        public static StarMessage Deserialize(string message)
        {
            return StarSerializer.Deserialize<StarMessage>(message);
        }
    }
}
