using System;
using System.IO;
using System.Net;

namespace Framework.Common.Network.Socket
{
    public class SocketNetwork : Singleton<SocketNetwork>, Interface.ISingleton
    {
        protected SocketIO socketIO = null;
        protected override void OnInitialize()
        {
            socketIO = new SocketIO(OnConnect, OnSocketClose, OnDataReceived, OnError);
        }
        protected override void OnDestroy()
        {
            if(socketIO?.IsConnected ?? false)
                socketIO.Disconnect();
            socketIO = null;
        }

        public bool Connect(string host, int port)
        {
            if (socketIO == null)
            {
                Debugger.Log("Socket is null");
                return false;
            }
            if (socketIO.IsConnected)
            {
                Debugger.Log("Socket is connected");
                return false;
            }
            try
            {
                socketIO.Connect(new DnsEndPoint(host, port));
                return true;
            }
            catch(Exception e)
            {
                Debugger.LogException(e);
            }
            return false;
        }
        protected virtual void OnConnect(object sender, EventArgs args)
        {
            SystemEvent.Instance.CallEvent(Enum.SocketType.Connected);
        }
        protected virtual void OnSocketClose(object sender, EventArgs args)
        {
            SystemEvent.Instance.CallEvent(Enum.SocketType.Disconnected);
        }
        protected virtual void OnError(object sender, ErrorEventArgs args)
        {
            SystemEvent.Instance.CallEvent(Enum.SocketType.Error);
        }
        protected virtual void OnDataReceived(object sender, DataEventArgs args)
        {
            SystemEvent.Instance.CallEvent(Enum.SocketType.DataReceive, args.Data);
        }

    }

}