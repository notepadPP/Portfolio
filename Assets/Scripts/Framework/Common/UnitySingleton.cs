using System;
using UnityEngine;
namespace Framework.Common
{
    public sealed class SingletonManager : MonoBehaviour
    {
        private static SingletonManager _instance = null;
        private static object _lock = new object();
        public static SingletonManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<SingletonManager>();
                        if (_instance == null)
                        {
                            GameObject go = new GameObject("SingletonManager");
                            _instance = go.AddComponent<SingletonManager>();
                            DontDestroyOnLoad(go);
                        }
                    }
                    return _instance;
                }
            }
        }
        void Awake() => DontDestroyOnLoad(gameObject);
        public static T CreateSingleton<T>() where T : Interface.ISingleton, new() => Instance.MakeSingleton<T>();
        private T MakeSingleton<T>() where T : Interface.ISingleton, new()
        {
            try
            {
                T singleton = new T();
                singleton.Initialize(this);
                return singleton;
            }
            catch (Exception e)
            {

            }
            return default;
        }
    }
}

