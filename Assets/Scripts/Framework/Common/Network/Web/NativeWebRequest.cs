using System;
using UnityEngine.Networking;

namespace Framework.Common.Network.Web
{
    public sealed class NativeWebRequest : Request
    {
        public Action<long, byte[]> callback { get; private set; } = null;
        private NativeWebRequest(UnityWebRequest request, Action<long, byte[]> callback) : base(request)
        {
            this.callback = callback;
        }
        public override void Invoke() => callback?.Invoke(responseCode, responseData);
        #region Make WebRequest
        public static NativeWebRequest MakeGetRequest(string url, string ContentType, Action<long, byte[]> callback, params (string, string)[] headers) => MakeRequest(url, ContentType, UnityWebRequest.kHttpVerbGET, null, callback, headers);
        public static NativeWebRequest MakePostRequest(string url, string ContentType, byte[] sendData, Action<long, byte[]> callback, params (string, string)[] headers) => MakeRequest(url, ContentType, UnityWebRequest.kHttpVerbPOST, sendData, callback, headers);

        private static NativeWebRequest MakeRequest(string url, string ContentType, string post, byte[] sendData, Action<long, byte[]> callback, params (string, string)[] headers)
        {
            UnityWebRequest req = MakeRequest(url, ContentType, post, sendData);
            if (headers != null)
            {
                for (int index = 0; index < headers.Length; ++index)
                {
                    var header = headers[index];
                    if (string.IsNullOrEmpty(header.Item1)) { Debugger.LogError($"No.{index} {nameof(header)} name"); continue; }
                    req.SetRequestHeader(header.Item1, header.Item2);
                }
            }
            return new NativeWebRequest(req, callback);
        }
        #endregion
    }
}