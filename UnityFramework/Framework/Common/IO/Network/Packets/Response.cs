using System;

namespace Framework.Common.IO.Network
{
    [Serializable]
    public class Response
    {
        public int resCode;
        public string resMsg;
        public object data;
    }
}
