using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Framework.Common.IO.Network
{
    public enum SocketMessageType
    {
        Connected,
        Disconnected,
        Error,   // Error only EventArgs type is ErrorEventArgs
        Receive, // Recieve only EventArgs type is DataEventArgs
    }

    public partial class NetworkManager : Template.Singleton<NetworkManager>
    {

        private static object socketLockObject = new object();

        public class SocketMessage
        {
            public string key;
            public EventArgs EventArgs;
        }
        protected class RecieveData
        {
            public SocketMessageType Type;
            public SocketMessage message;
        }
        Dictionary<string, Socket.SocketIO> Sockets = new Dictionary<string, Socket.SocketIO>();
        Queue<RecieveData> SocketReceiveDatas = new Queue<RecieveData>();
        Coroutine SocketReceiveCoroutine = null;
        public void SocketConnect(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Not Exist Server: " + key);
                return;
            }
            if (Sockets.TryGetValue(key, out Socket.SocketIO socket) == false)
            {
                string address = GetServer(key);
                if (string.IsNullOrEmpty(address))
                {
                    Debug.LogError("Not Exist Server: " + key);
                    return;
                }
                if (TryGetAddress(address, out string host, out int port) == false)
                {
                    Debug.LogError("Invalid Address: " + address);
                    return;
                }
                socket = new Socket.SocketIO(OnSocketConnected, OnSocketDisconnected, OnSocketReceive, OnSocketError);
                socket.Connect(new DnsEndPoint(host, port));
                Sockets.Add(key, socket);
            }
            else
            {
                Debug.Log(GetServer(key) + " Already Connected");
            }
        }
        public void SocketSend(SocketRequest request)
        {
            if (request == null)
            {
                Debug.LogError("Invalid Request");
                return;
            }
            SocketSend(request.serverKey, request.bytes);
        }
        public void SocketSend(string key, byte[] bytes)
        {
            if (Sockets.TryGetValue(key, out Socket.SocketIO socket) == false)
            {
                Debug.LogError("Not Connected Server: " + key);
                return;
            }
            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogError("Invalid Bytes");
                return;
            }
            socket.Send(bytes, 0, bytes.Length);
        }

        private string GetSocketKey(Socket.SocketIO socket)
        {
            if (socket == null) return string.Empty;

            foreach (KeyValuePair<string, Socket.SocketIO> pair in Sockets)
            {
                if (pair.Value == socket)
                {
                    return pair.Key;
                }
            }
            return string.Empty;

        }
        private void OnSocketConnected(object sender, System.EventArgs e)
        {

            string key = GetSocketKey(sender as Socket.SocketIO);
            if (key == string.Empty)
            {
                Debug.LogError("Not Exist Socket: " + sender);
                return;
            }
            ReceiveData(SocketMessageType.Connected, key, e);
        }
        private void OnSocketDisconnected(object sender, System.EventArgs e)
        {
            string key = GetSocketKey(sender as Socket.SocketIO);
            if (key == string.Empty)
            {
                Debug.LogError("Not Exist Socket: " + sender);
                return;
            }
            ReceiveData(SocketMessageType.Disconnected, key, e);
        }
        private void OnSocketError(object sender, System.EventArgs e)
        {
            string key = GetSocketKey(sender as Socket.SocketIO);
            if (key == string.Empty)
            {
                Debug.LogError("Not Exist Socket: " + sender);
                return;
            }
            ReceiveData(SocketMessageType.Error, key, e);
        }
        private void OnSocketReceive(object sender, System.EventArgs e)
        {
            string key = GetSocketKey(sender as Socket.SocketIO);
            if (key == string.Empty)
            {
                Debug.LogError("Not Exist Socket: " + sender);
                return;
            }
            ReceiveData(SocketMessageType.Receive, key, e);
        }

        private void ReceiveData(SocketMessageType type, string key, EventArgs e)
        {
            lock (socketLockObject)
            {
                SocketReceiveDatas.Enqueue(new RecieveData()
                {
                    Type = type,
                    message = new SocketMessage()
                    {
                        key = key,
                        EventArgs = e,
                    }
                });
            }
            if (SocketReceiveCoroutine == null)
                SocketReceiveCoroutine = StartCoroutine(ReceiveAsyc());
        }
        private IEnumerator ReceiveAsyc()
        {
            while (SocketReceiveDatas.Count > 0)
            {
                lock (socketLockObject)
                {
                    RecieveData data = SocketReceiveDatas.Dequeue();
                    this.Dispatch(data.Type, data.message);
                }
                yield return null;
            }
            SocketReceiveCoroutine = null;
        }


        private static bool TryGetAddress(string address, out string host, out int port)
        {
            host = string.Empty;
            port = 0;
            try
            {
                if (Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out Uri uri))
                { 
                    host = uri.Host;
                    port = uri.Port;
                    return true;
                }
                else
                {
                    string[] split = address.Split(':');
                    if (split.Length == 2)
                    {
                        host = split[0];
                        port = int.Parse(split[1]);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return false;
        }
    }
}