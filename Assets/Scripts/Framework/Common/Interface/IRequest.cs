#if UNITY_2012_2
using System.Threading.Tasks;
#else
using System.Collections;
#endif



namespace Framework.Common.Network.Web.Interface
{
    public interface IRequest
    {
        bool isSend { get; }
        long responseCode { get; }
        bool isError { get; }
        string errorString { get; }
        byte[] responseData { get; }
        string URL { get; }
        void Send();
        void Disponse();
#if UNITY_2012_2
        async Task SendAync();
#else
        IEnumerator SendAync();
#endif
    }
}