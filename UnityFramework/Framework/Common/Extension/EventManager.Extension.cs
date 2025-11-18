using Framework.Common.Events;
using Framework.Common.Template;
using System;
using UnityEngine.Events;

namespace Framework.Common
{

    public static partial class Extension
    {
        public static void AddListener<T>(this Interface.IElement element, T type, UnityAction<object> action) where T : Enum
        {
            if (element is SingletonBase)
                EventManager.Instance.AddListener(type, action, EventManager.EventType.Singleton);
            else
                EventManager.Instance.AddListener(type, action, EventManager.EventType.Default);
        }
        public static void AddListener<T>(this Interface.IElement element, T type, Func<object, bool> action) where T : Enum
        {
            if (element is SingletonBase)
                EventManager.Instance.AddListener(type, action, EventManager.EventType.Singleton);
            else
                EventManager.Instance.AddListener(type, action, EventManager.EventType.Default);
        }
        public static void RemoveListener<T>(this Interface.IElement element, T type, bool isFunc) where T : Enum
        {
            if(isFunc)
                element.RemoveListener(type, null as UnityAction<object>);
            else
                element.RemoveListener(type, null as Func<object, bool>);
        }
        public static void RemoveListener<T>(this Interface.IElement element, T type, UnityAction<object> action = null) where T : Enum
        {
            if (element is SingletonBase)
                EventManager.Instance.RemoveListener(type, action, EventManager.EventType.Singleton);
            else
                EventManager.Instance.RemoveListener(type, action, EventManager.EventType.Default);
        }
        public static void RemoveListener<T>(this Interface.IElement element, T type, Func<object, bool> func = null) where T : Enum
        {
            if (element is SingletonBase)
                EventManager.Instance.RemoveListener(type, func, EventManager.EventType.Singleton);
            else
                EventManager.Instance.RemoveListener(type, func, EventManager.EventType.Default);
        }
        public static void Dispatch<T>(this Interface.IElement element, T type, object obj = null) where T : Enum
        {
            if (element != null)
                EventManager.Instance.Dispatch(type, obj);
        }
    }
}