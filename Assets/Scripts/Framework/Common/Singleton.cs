using System;
using UnityEngine;

namespace Framework.Common
{
    public abstract class Singleton<T> : LifeCycle where T : Interface.ISingleton, new()
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
