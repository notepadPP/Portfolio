using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Common.Network.Web
{
    public class WebNetwork : Singleton<WebNetwork>, Framework.Common.Interface.ISingleton
    {
        Queue<Request> requests = null;
        Coroutine coroutine = null;
        protected override void OnInitialize()
        {
            requests = new Queue<Request>();
        }
        protected override void OnDestroy()
        {
        }

        private void SendRequest(Request webRequest)
        {
            requests.Enqueue(webRequest);
            if (coroutine == null)
                coroutine = behaviour.StartCoroutine(CoRequestUpdate());
        }
        IEnumerator CoRequestUpdate()
        {
            while (requests.Count > 0)
            {
                Request info = requests.Dequeue();
                if (info == null) continue;
                yield return info.SendAync();
                if (info.IsError)
                    Debugger.LogError($"RECV : E_FAIL {nameof(info.URL)}.{info.URL}, {nameof(info.ResponseCode)}.{info.ResponseCode}, {info.ErrorString}");
                else
                    Debugger.Log($"RECV : S_OK {nameof(info.URL)}.{info.URL}, {nameof(info.ResponseCode)}.{info.ResponseCode}");
                try
                {
                    info.Invoke();
                }
                catch (Exception e) { Debugger.LogException(e); }
            }
            behaviour.StopCoroutine( coroutine);
            coroutine = null;
        }
    }
}