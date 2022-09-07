using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Framework.Common.Network.Socket
{
    public class SocketIO
    {
        EventHandler Connected;
        EventHandler Closed;
        EventHandler<ErrorEventArgs> Error;
        EventHandler<DataEventArgs> DataReceived;
        #region Only Read
        public bool IsConnected => SocketEventArgsList != null && SocketEventArgsList.Count > 0 && SocketEventArgsList[0] != null && SocketEventArgsList[0].ConnectSocket != null && SocketEventArgsList[0].ConnectSocket.Connected;
        #endregion
        public int ReceiveBufferSize = 4096;
        public int Timeout = 600;
        private List<SocketAsyncEventArgs> SocketEventArgsList = null;
        private SocketIO()
        {
            SocketEventArgsList = new List<SocketAsyncEventArgs>();

        }
        public SocketIO(EventHandler connected, EventHandler closed, EventHandler<DataEventArgs> dataReceived, EventHandler<ErrorEventArgs> error) : this()
        {
            Connected = connected;
            Closed = closed;
            Error = error;
            DataReceived = dataReceived;
        }
        public void Connect(EndPoint remoteEndPoint)
        {
            SocketAsyncEventArgs SocketEventArgs = MakeSocketAsyncEventArgs(remoteEndPoint, OnSocketConnectEventArgsCompleted);
            System.Net.Sockets.Socket socket = MakeSocket();
            socket?.ConnectAsync(SocketEventArgs);
        }

        public void Disconnect(bool isAsync = false)
        {
            try
            {
                if (SocketEventArgsList.Count <= 0) return;
                SocketAsyncEventArgs SocketEventArgs = SocketEventArgsList[0];
                if (SocketEventArgs == null ||
                    SocketEventArgs.ConnectSocket == null ||
                    SocketEventArgs.ConnectSocket.Connected == false)
                    return;

                if (isAsync)
                {
                    SetSocketAsyncEventCompleted(SocketEventArgs, OnSocketDisconnectEventArgsCompleted);
                    SocketEventArgs.ConnectSocket.DisconnectAsync(SocketEventArgs);
                }
                else
                {
                    SocketEventArgs.Completed -= OnSocketConnectEventArgsCompleted;
                    SocketEventArgs.Completed -= OnSocketDisconnectEventArgsCompleted;
                    SocketEventArgs.Completed -= OnSocketSendEventArgsCompleted;
                    SocketEventArgs.Completed -= OnSocketReceiveEventArgsCompleted;
                    Closed.Invoke(SocketEventArgs.ConnectSocket, SocketEventArgs);
                    SocketEventArgs.ConnectSocket.Shutdown(SocketShutdown.Both);
                    SocketEventArgs.ConnectSocket.Close();
                }

                SocketEventArgsList.Remove(SocketEventArgs);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void Send(byte[] data, int offset, int length)
        {
            try
            {
                if (SocketEventArgsList.Count <= 0) return;
                SocketAsyncEventArgs SocketEventArgs = SocketEventArgsList[0];
                if (SocketEventArgs == null || SocketEventArgs.ConnectSocket == null || SocketEventArgs.ConnectSocket.Connected == false) return;
                SocketAsyncEventArgs SocketSendEventArgs = MakeSocketAsyncEventArgs(data, offset, length, OnSocketSendEventArgsCompleted);
                SocketEventArgs.ConnectSocket.SendTimeout = Timeout;
                SocketEventArgs.ConnectSocket.SendAsync(SocketSendEventArgs);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        private void Receive(SocketAsyncEventArgs e)
        {
            if (e == null || e.ConnectSocket == null) return;
            try
            {
                e.ConnectSocket.ReceiveTimeout = Timeout;
                bool flag = e.ConnectSocket.ReceiveAsync(e);
                if (!flag) ProcessReceive(e);
            }
            catch (Exception ex)
            {
                if (SendException(ex)) return;
            }
        }
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (IsSocketError(e) || IsBytesTransferred(e)) return;
            OnDataReceived(e.Buffer, e.Offset, e.BytesTransferred);
            Receive(e);
        }
        private SocketAsyncEventArgs MakeSocketAsyncEventArgs(EventHandler<SocketAsyncEventArgs> e)
        {
            SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs();
            SetSocketAsyncEventCompleted(socketEventArgs, e);
            return socketEventArgs;
        }
        private SocketAsyncEventArgs MakeSocketAsyncEventArgs(byte[] data, int offset, int length, EventHandler<SocketAsyncEventArgs> e)
        {
            SocketAsyncEventArgs socketEventArgs = MakeSocketAsyncEventArgs(e);
            if (data != null && data.Length > 0)
                socketEventArgs.SetBuffer(data, offset, length);

            return socketEventArgs;
        }
        private SocketAsyncEventArgs MakeSocketAsyncEventArgs(EndPoint remoteEndPoint, EventHandler<SocketAsyncEventArgs> e)
        {
            SocketAsyncEventArgs socketEventArgs = MakeSocketAsyncEventArgs(e);
            if (remoteEndPoint != null)
                socketEventArgs.RemoteEndPoint = remoteEndPoint;
            return socketEventArgs;
        }
        private static System.Net.Sockets.Socket MakeSocket() => PreferIPv4Stack() ?
                                              new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) :
                                              new System.Net.Sockets.Socket(SocketType.Stream, ProtocolType.Tcp);
        private static bool PreferIPv4Stack() => Environment.GetEnvironmentVariable("PREFER_IPv4_STACK") != null;
        private void SetSocketAsyncEventCompleted(SocketAsyncEventArgs SocketEventArgs, EventHandler<SocketAsyncEventArgs> e)
        {
            SocketEventArgs.Completed -= OnSocketConnectEventArgsCompleted;
            SocketEventArgs.Completed -= OnSocketDisconnectEventArgsCompleted;
            SocketEventArgs.Completed -= OnSocketSendEventArgsCompleted;
            SocketEventArgs.Completed -= OnSocketReceiveEventArgsCompleted;
            SocketEventArgs.Completed += e;
        }
        #region Error
        private bool IsIgnorableSocketError(SocketError errorCode) => errorCode == SocketError.Shutdown ||
                                                                        errorCode == SocketError.ConnectionAborted ||
                                                                        errorCode == SocketError.ConnectionReset ||
                                                                        errorCode == SocketError.OperationAborted;
        private bool SendException(Exception e)
        {
            if (e == null) return false;
            if (e is ObjectDisposedException ||
                e is NullReferenceException ||
                (e is SocketException ex && !IsIgnorableSocketError(ex.SocketErrorCode)))
            {
                OnError(e);
            }
            Disconnect(true);
            return true;
        }
        private bool IsBytesTransferred(SocketAsyncEventArgs e)
        {
            if (e == null || e.BytesTransferred == 0)
            {
                Disconnect(true);
                return true;
            }
            return false;
        }
        private bool IsSocketError(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                OnError(new SocketException((int)SocketError.InvalidArgument));
                return true;
            }
            if (e.SocketError != SocketError.Success && !IsIgnorableSocketError(e.SocketError))
            {
                OnError(new SocketException((int)e.SocketError));
                return true;
            }
            return false;
        }
        #endregion
        #region Completed
        private void OnSocketDisconnectEventArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (SocketEventArgsList.Count <= 0 || SocketEventArgsList[0] == e)
                Closed?.Invoke(this, e);
            e.ConnectSocket?.Close();
            //socket = null;
        }
        private void OnSocketConnectEventArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (IsSocketError(e))
            {
                e.Dispose();
                return;
            }
            if (e == null)
            {
                OnError(new SocketException((int)SocketError.ConnectionAborted));
                return;
            }
            if (!e.ConnectSocket.Connected)
            {
                try
                {
                    SocketError socketError = (SocketError)(e.ConnectSocket?.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error) ?? SocketError.ConnectionAborted);
                    OnError(new SocketException((int)socketError));
                }
                catch (Exception)
                {
                    OnError(new SocketException((int)SocketError.HostUnreachable));
                }
                return;
            }
            SetSocketAsyncEventCompleted(e, OnSocketReceiveEventArgsCompleted);
            if (e.Buffer == null)
            {
                ArraySegment<byte> Buffer = new ArraySegment<byte>(new byte[ReceiveBufferSize]);
                e.SetBuffer(Buffer.Array, Buffer.Offset, Buffer.Count);
            }
            else
                e.SetBuffer(0, ReceiveBufferSize);
            SocketEventArgsList.Add(e);
            Connected?.Invoke(this, e);
            Receive(e);
        }
        private void OnSocketSendEventArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (IsSocketError(e) || IsBytesTransferred(e)) return;
        }
        private void OnSocketReceiveEventArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }
        #endregion
        #region Send Event
        private void OnError(Exception e) => Error?.Invoke(this, new ErrorEventArgs(e));
        private void OnClosed() => Closed?.Invoke(this, EventArgs.Empty);
        private void OnDataReceived(byte[] data, int offset, int length)
        {

            DataEventArgs dataArgs = new DataEventArgs
            {
                Data = data,
                Offset = offset,
                Length = length
            };
            DataReceived?.Invoke(this, dataArgs);
        }
        #endregion
    }
}