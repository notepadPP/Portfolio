using UnityEngine;

namespace Framework.Common.Interface
{
    public interface ISingleton : ICreatable
    {
        void Initialize(MonoBehaviour behaviour);
    }
}