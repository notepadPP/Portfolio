using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Common.IO.Network
{
    public interface IRequestData
    {
    }
    public interface IWWWForm
    {
        public WWWForm GetWWWForm();
    }
    public abstract class RequestDataBase : IRequestData, IWWWForm
    {

        public virtual WWWForm GetWWWForm() => null;
        public override string ToString() => JsonUtility.ToJson(this);
    }

    public class RequestFile 
    {
        public string fileName;
        public byte[] fileData;
        public string mimeType;
    }

}
