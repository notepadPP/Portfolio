using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Common.IO.Network.Web
{
    public enum Result
    {
        Unknown,
        InProgress,
        Success,
        ConnectionError,
        ProtocolError,
        DataProcessingError,
        ServerError,
    }
    public class WebRequest
    {
        public string Error => request?.error ?? string.Empty;
        public string ResponseText => request.downloadHandler.text;
        public byte[] ResponseData => request.downloadHandler.data;
        public DownloadHandler downloadHandler => request?.downloadHandler ?? null;
        public virtual Framework.Common.IO.Network.Web.Result result => GetResult();
        UnityWebRequest request;
        public WebRequest(string url, WWWForm form, params (string, string)[] headers)
        {
            bool isPost = form != null;
            if (isPost)
                request = UnityWebRequest.Post(url, form);
            else
                request = UnityWebRequest.Get(url);

            if (headers == null) return;
            foreach (var header in headers)
                request.SetRequestHeader(header.Item1, header.Item2);
        }
        public WebRequest(string url, string text, params (string, string)[] headers)
        {
            bool isPost = string.IsNullOrEmpty(text) == false;
            if (isPost)
                request = UnityWebRequest.PostWwwForm(url, text);
            else
                request = UnityWebRequest.Get(url);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(text));
            request.downloadHandler = new DownloadHandlerBuffer();
            if (headers == null) return;
            foreach (var header in headers)
                request.SetRequestHeader(header.Item1, header.Item2);
        }
        public WebRequest(string url, bool isPost, object obj, params (string, string)[] values)
        {
            string json = JsonUtility.ToJson(obj);
            if (isPost)
                request = UnityWebRequest.PostWwwForm(url, json);
            else
                request = UnityWebRequest.Get(url);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            foreach (var value in values)
                request.SetRequestHeader(value.Item1, value.Item2);
        }
        public UnityWebRequestAsyncOperation Send()
        {
            try
            {
                return request.SendWebRequest();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return null;
        }
        public void Dispose() => request?.Dispose();
        private Result GetResult()
        {
            try
            {
                switch (request.result)
                {
                    case UnityWebRequest.Result.Success: return Result.Success;
                    case UnityWebRequest.Result.InProgress: return Result.InProgress;
                    case UnityWebRequest.Result.ConnectionError: return Result.ConnectionError;
                    case UnityWebRequest.Result.DataProcessingError: return Result.DataProcessingError;
                    default: return Result.Unknown;
                    case UnityWebRequest.Result.ProtocolError:
                        {
                            Debug.Log($"HTTP/1.1 {request.responseCode} {request.error}");
                            return Result.ProtocolError;
                        }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return Result.Unknown;
            }
        }
        
    }

}
