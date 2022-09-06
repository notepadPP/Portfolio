using System;
using UnityEngine;

namespace Common
{
    public abstract class Singleton<T> : LifeCycle where T : ISingleton, new()
    {

        private static readonly Lazy<T> _instance = new Lazy<T>(() => SingletonManager.CreateSingleton<T>());
        public static T Instance => _instance.Value;

        protected MonoBehaviour behaviour = null;
        public virtual void Initialize(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
            Initialize();
        }

    }

}
