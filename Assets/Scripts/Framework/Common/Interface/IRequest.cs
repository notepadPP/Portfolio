using System.Collections;

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
        IEnumerator SendAync();
    }
}