using Common;
using Common.Helper;
using System;
using System.Collections.Generic;

public class SystemEvent : Singleton<SystemEvent>, ISingleton
{
    Dictionary<(long, Type), Action<object>> events = null;
    private (long, Type) GetKey<T>(T id) where T : unmanaged, Enum => (id.ToLong(), typeof(T));
    public void BindEvent<T>(T type, Action<object> action) where T : unmanaged, Enum
    {
        var key = GetKey(type);
        if (events.ContainsKey(key) == false) events.Add(key, action);
        else events[key] += action;
    }
    public void UnbindEvent<T>(T type, Action<object> action = null) where T : unmanaged, Enum
    {
        var key = GetKey(type);
        if (events.ContainsKey(key) == false) return;
        if (action == null)
            events.Remove(key);
        else
            events[key] -= action;
    }
    public void UnbindEvent<T>() where T : unmanaged, Enum
    {
        foreach (T id in Enum.GetValues(typeof(T)))
            UnbindEvent(id);
    }
    public void CallEvent<T>(T type, object obj = null) where T : unmanaged, Enum
    {
        var key = GetKey(type);
        if (events.ContainsKey(key) == false) return;
        events[key]?.Invoke(obj);

    }

    protected override void OnDestroy()
    {
        events.Clear();
    }

    protected override void OnInitialize()
    {
        events = new Dictionary<(long, Type), Action<object>>();
    }
    
    

}
