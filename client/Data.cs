// NOTE: THIS FILE MUST NOT CHANGE

using System.Text.Json;

namespace LibData
{

    public class Message
    {
        public int MsgId { get; set; }
        public MessageType MsgType { get; set; }
        public Object? Content { get; set; }
    }

    public enum MessageType
    {
        Hello,
        Welcome,
        DNSLookup,
        DNSLookupReply,
        DNSRecord,
        Ack,
        End,
        Error
    }

    public class DNSRecord
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
        public int? TTL { get; set; }
        public int? Priority { get; set; } // Nullable for non-MX records
    }
}
