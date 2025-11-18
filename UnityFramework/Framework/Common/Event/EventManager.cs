using Framework.Common.Template;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework.Common.Events
{

    using ActionEventDictionary = Dictionary<(string, Type), UnityEvent<object>>;
    using FunctionEventDictionary = Dictionary<(string, Type), List<Func<object, bool>>>;
    public class EventManager : Singleton<EventManager>
    {
        public enum EventType
        {
            Singleton,
            Default,
        }
        Dictionary<EventType, ActionEventDictionary> ActionEvents = new Dictionary<EventType, ActionEventDictionary>();
        Dictionary<EventType, FunctionEventDictionary> FuncEvents = new Dictionary<EventType, FunctionEventDictionary>();
        Array eventTypes = null;
        public override void DoDestroy()
        {
        }
        public override void OnInitialize()
        {
            eventTypes = Enum.GetValues(typeof(EventType));
        }

        public void Clear(EventType eventType)
        {
            if (TryGetActionEventDictionary(eventType, out ActionEventDictionary actionEvents) == false) return;
            actionEvents.Clear();
            if (TryGetFunctionEventDictionary(eventType, out FunctionEventDictionary funcEvents) == false) return;
            funcEvents.Clear();
        }
        #region UnityEvent Listener Add/Remove ( this Add/Remove is call all notifications)
        public void AddListener<T>(T type, UnityAction<object> action, EventType eventType = EventType.Default) where T : Enum
        {
            if (TryGetActionEventDictionary(eventType, out ActionEventDictionary events) == false)
            {
                events = new ActionEventDictionary();
                ActionEvents.Add(eventType, events);
            }
            var key = GetKey(type);
            if (events.TryGetValue(key, out UnityEvent<object> Event) == false)
                events.Add(key, Event = new UnityEvent<object>());
            Event.AddListener(action);
        }
        public void RemoveListener<T>(T type, UnityAction<object> action, EventType eventType = EventType.Default) where T : Enum
        {
            if (TryGetActionEventDictionary(eventType, out ActionEventDictionary events) == false)
                return;
            var key = GetKey(type);
            if (events.TryGetValue(key, out UnityEvent<object> Event))
            {
                if (action != null)
                    Event.RemoveListener(action);
                else
                    events.Remove(key);
            }
        }
        #endregion
        #region Func Listener Add/Remove ( this Add/Remove is call func is selected)
        public void AddListener<T>(T type, Func<object, bool> action, EventType eventType = EventType.Default) where T : Enum
        {
            if (TryGetFunctionEventDictionary(eventType, out FunctionEventDictionary events) == false)
            {
                events = new FunctionEventDictionary();
                FuncEvents.Add(eventType, events);
            }
            var key = GetKey(type);
            if (events.TryGetValue(key, out List<Func<object, bool>> Events) == false)
                events.Add(key, Events = new List<Func<object, bool>>());
            if (Events.Contains(action) == false)
                Events.Add(action);
        }
        public void RemoveListener<T>(T type, Func<object, bool> action, EventType eventType = EventType.Default) where T : Enum
        {
            if (TryGetFunctionEventDictionary(eventType, out FunctionEventDictionary events) == false) return;

            var key = GetKey(type);
            if (events.TryGetValue(key, out List<Func<object, bool>> Events) == false) return;
            if (action != null)
            {
                if (Events.Contains(action) == true)
                    Events.Remove(action);
            }
            else
            {
                events.Remove(key);
            }
        }
        #endregion

        public void Dispatch<T>(T type, object obj = null) where T : Enum
        {
            var key = GetKey(type);
            foreach (EventType eventType in eventTypes)
                ActionEventDictionaryDispatch(eventType, key, obj);
            foreach (EventType eventType in eventTypes)
            {
                if (FunctionEventDictionaryDispatch(eventType, key, obj) == false)
                    break;
            }
        }

        private static (string, Type) GetKey<T>(T key) where T : Enum => (key.ToString(), key.GetType());
        private void ActionEventDictionaryDispatch(EventType eventType, (string, Type) key, object obj)
        {
            if (TryGetActionEventDictionary(eventType, out ActionEventDictionary dictionary) == false)
                return;

            if (dictionary.TryGetValue(key, out UnityEvent<object> Event))
                Event.Invoke(obj);
        }
        private bool FunctionEventDictionaryDispatch(EventType eventType, (string, Type) key, object obj)
        {
            if (TryGetFunctionEventDictionary(eventType, out FunctionEventDictionary dictionary) == false)
                return true;
            if (dictionary.TryGetValue(key, out List<Func<object, bool>> Events) == false)
                return true;
            foreach (Func<object, bool> func in Events)
            {
                if (func.Invoke(obj) == false)
                    return false;
            }
            return true;
        }
        private bool TryGetActionEventDictionary(EventType eventType, out ActionEventDictionary dictionary)
        {
            if (ActionEvents.TryGetValue(eventType, out dictionary) == false)
                ActionEvents.Add(eventType, dictionary = new ActionEventDictionary());
            return true;
        }
        private bool TryGetFunctionEventDictionary(EventType eventType, out FunctionEventDictionary dictionary)
        {
            if (FuncEvents.TryGetValue(eventType, out dictionary) == false)
                FuncEvents.Add(eventType, dictionary = new FunctionEventDictionary());
            return true;
        }
    }

}