using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Common
{
    public sealed class SingletonManager : Template.MonoSingleton<SingletonManager>
    {
#if UNITY_EDITOR
        [SerializeField] List<string> singletonList = null;
#endif
        Dictionary<Type, Template.SingletonBase> Singletons = null;
        public override void DoDestroy()
        {
            if (Singletons == null) return;
            foreach (Template.SingletonBase singleton in Singletons.Values)
            {
                singleton.Destroy();
            }
            Singletons.Clear();
#if UNITY_EDITOR
            singletonList.Clear();
#endif
        }

        public override void OnInitialize()
        {
#if UNITY_EDITOR
            singletonList = new List<string>();
#endif
            Singletons = new Dictionary<Type, Template.SingletonBase>();
        }
        public T AddSingleton<T>() where T : Template.SingletonBase, new()
        {
            Type type = typeof(T);
            if (Singletons.TryGetValue(type, out Template.SingletonBase singletonBase))
                return singletonBase as T;
            singletonBase = new T();
            Singletons.Add(type, singletonBase);
            singletonBase.SetMonoBehaviour(this);
            singletonBase.Initialize();
#if UNITY_EDITOR
            singletonList.Add(type.FullName);
#endif
            return singletonBase as T;

        }
    }
}