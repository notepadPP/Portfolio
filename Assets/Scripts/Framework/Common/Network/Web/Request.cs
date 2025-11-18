using UnityEngine.Networking;
#if UNITY_2012_2
using System.Threading.Tasks;
#else
using System.Collections;
#endif


namespace Framework.Common.Network.Web
{
    public abstract class Request : Interface.IRequest
    {
        private UnityWebRequest request = null;
        protected Request(UnityWebRequest request) => this.request = request;
        public bool isDone => request?.isDone ?? false;
        public bool isSend { get; private set; } = false;
        public long responseCode => request?.responseCode ?? 0;
#if !UNITY_2020_2_OR_NEWER
        public bool isError => request.isNetworkError || request.isHttpError;
#else
        public bool isError => request.result != UnityWebRequest.Result.InProgress && request.result != UnityWebRequest.Result.Success;
#endif
        public string errorString => request.error;
        public byte[] responseData => request.downloadHandler.data;
        public string URL { get; }

        public void Send()
        {
            request?.SendWebRequest();
            isSend = true;
        }
        public void Disponse()
        {
            request.Dispose();
            request = null;
        }
#if UNITY_2012_2
        async public Task SendAync()
        {
            IsSend = true;
            yield return request.SendWebRequest();
        }
#else
        public IEnumerator SendAync()
        {
            isSend = true;
            yield return request.SendWebRequest();
        }
#endif


        public abstract void Invoke();

        #region Make UnityWebrequest
        protected static UnityWebRequest MakeRequest(string url, string ContentType, string post, byte[] sendData)
        {
            UnityWebRequest req = new UnityWebRequest(url, post)
            {
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = (sendData == null || sendData.Length < 1) ? null : new UploadHandlerRaw(sendData) { contentType = ContentType },
            };
            return req;
        }

        #endregion
    }
}