using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : LifeCycle where T : class, new()
{
    //Queue<T> queue = null;
    //public T Get()
    //{
    //    queue
    //    //T 
    //}
    protected override void OnDestroy()
    {
    }

    protected override void OnInitialize()
    {
        //queue = new Queue<T>();
    }

}
