using UnityEngine;

namespace Common
{
    public interface ISingleton : ICreatable
    {
        void Initialize(MonoBehaviour behaviour);
    }
}