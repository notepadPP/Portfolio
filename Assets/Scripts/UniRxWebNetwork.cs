using Framework.Common;
using Framework.Common.Network.Web;
using System;
using UniRx;

public class UniRxWebNetwork : Singleton<UniRxWebNetwork>, Framework.Common.Interface.ISingleton
{
    AsyncSubject<Request> asyncSubject = null;
    protected override void OnInitialize()
    {
        asyncSubject = new AsyncSubject<Request>();
        asyncSubject.Subscribe(req => RequestUpdate(req));
    }
    protected override void OnDestroy()
    {
        asyncSubject = null;
    }
    public void SendRequest(Request webRequest)
    {
        asyncSubject.OnNext(webRequest);
    }

    private void RequestUpdate(Request request)
    {
        if (request == null) return;
        request.Send();

        while (request.isDone) { }

        if (request.isError)
            Debugger.LogError($"RECV : E_FAIL {nameof(request.URL)}.{request.URL}, {nameof(request.responseCode)}.{request.responseCode}, {request.errorString}");
        else
            Debugger.Log($"RECV : S_OK {nameof(request.URL)}.{request.URL}, {nameof(request.responseCode)}.{request.responseCode}");
        try
        {
            request.Invoke();
        }
        catch (Exception e) { Debugger.LogException(e); }

    }
}