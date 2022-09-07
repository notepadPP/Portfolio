using System;

namespace Framework.Common.Network.Socket
{
    public class DataEventArgs : EventArgs
    {
        public byte[] Data;
        public int Offset;
        public int Length;
    }
}