using System.Collections;
using UnityEngine.Networking;

namespace Framework.Common.Network.Web
{
    public abstract class Request : Interface.IRequest
    {
        private UnityWebRequest request = null;
        protected Request(UnityWebRequest request) => this.request = request;
        public bool IsSend { get; private set; } = false;
        public long ResponseCode => request?.responseCode ?? 0;
        public bool IsError => request.isNetworkError || request.isHttpError;
        public string ErrorString => request.error;
        public byte[] ResponseData => request.downloadHandler.data;
        public string URL { get; }

        public void Send()
        {
            request?.SendWebRequest();
            IsSend = true;
        }
        public void Disponse()
        {
            request.Dispose();
            request = null;
        }
        public IEnumerator SendAync()
        {
            IsSend = true;
            yield return request.SendWebRequest();
        }
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