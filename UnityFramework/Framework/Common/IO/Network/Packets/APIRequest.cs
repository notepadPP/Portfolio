using System;

namespace Framework.Common.IO.Network
{
    public abstract class APIRequest : IRequest
    {
        public abstract Enum sendType { get; }
        static protected (string, string)[] JsonHeader = new (string, string)[] { ("Content-Type", "application/json"), ("Accept", "application/json"), ("Accept-Charset", "utf-8") };
        static protected (string, string)[] FileHeader = null;
        public abstract (string, string)[] headers { get; }
        public string Url => $"{serverKey}/{apiPath}";
        public abstract string serverKey { get; }
        public abstract string apiPath { get; }
        public RequestDataBase data { get; protected set; }
    }
}
