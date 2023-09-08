using Framework.Common.IO.Network.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Common.IO.Network
{
    public partial class NetworkManager
    {
        protected class RequestData
        {
            public APIRequest Request;
            public Type responseType;
        }
        private static object webLockObject = new object();
        List<RequestData> APIDatas = new List<RequestData>();

        Coroutine WebReceiveCoroutine = null;
        public void APISend<T>(APIRequest request) where T : class, IResponseData
        {

            try
            {
                lock (webLockObject)
                {
                    RequestData data = new RequestData()
                    {
                        Request = request,
                        responseType = typeof(T),
                    };
                    RequestProcess(data);
                    APIDatas.Add(data);
                }
                if (WebReceiveCoroutine == null)
                    WebReceiveCoroutine = behaviour.StartCoroutine(SendRequest());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        IEnumerator SendRequest()
        {
            while (APIDatas.Count > 0)
            {
                WebRequest request = null;
                RequestData data = null;
                lock (webLockObject)
                {
                    data = APIDatas[0];
                    APIDatas.RemoveAt(0);
                }
                if (ServerList.TryGetValue(data.Request.serverKey, out string address) == false)
                {
                    Debug.LogError("Not Exist Server: " + data.Request.serverKey);
                    this.Dispatch(data.Request.sendType, null);
                    yield break;
                }

                WWWForm form = data.Request.data.GetWWWForm();
                if (form != null)
                    request = new WebRequest($"{address}/{data.Request.apiPath}", form, data.Request.headers);
                else
                    request = new WebRequest($"{address}/{data.Request.apiPath}", data.Request.data.ToString(), data.Request.headers);

                if (request == null)
                {
                    Debug.LogError("Error While Sending: " + data.Request.apiPath);
                    yield break;
                }
                yield return request.Send();
                IResponseData responseData = null;
                try
                {
                    //responseData = ResponseProcess(data, request);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    this.Dispatch(data.Request.sendType, responseData);
                }

            }
        }
        private void RequestProcess(RequestData data)
        {
            if (data == null)
                throw new Exception("RequestData is Null");
            if (data.Request == null)
                throw new Exception("Request is Null");
            if (data.Request.data == null)
                throw new Exception("RequestData is Null");
            if (data.Request.sendType == null)
                throw new Exception("sendType is Null");
            if (data.responseType == null)
                throw new Exception("responseType is Null");
            if (string.IsNullOrEmpty(data.Request.serverKey))
                throw new Exception("serverKey is Null");
            if (string.IsNullOrEmpty(data.Request.apiPath))
                throw new Exception("apiPath is Null");
        }
    }
}