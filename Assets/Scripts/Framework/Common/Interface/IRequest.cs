#if UNITY_2012_2
using System.Threading.Tasks;
#else
using System.Collections;
#endif



namespace Framework.Common.Network.Web.Interface
{
    public interface IRequest
    {
        bool IsSend { get; }
        long ResponseCode { get; }
        bool IsError { get; }
        string ErrorString { get; }
        byte[] ResponseData { get; }
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