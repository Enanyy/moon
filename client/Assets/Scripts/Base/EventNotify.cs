using System.Collections.Generic;
using UnityEngine.Events;
using System;

/// <summary>
/// 事件派发器
/// </summary>
public class EventNotify
{
    #region Listener
    interface IEventListener
    {

    }
    class EventListener : UnityEvent, IEventListener
    {
        public EventListener() { }
    }
    class EventListener<T0> : UnityEvent<T0>, IEventListener
    {
        public EventListener() { }
    }
    class EventListener<T0, T1> : UnityEvent<T0, T1>, IEventListener
    {
        public EventListener() { }
    }
    class EventListener<T0, T1, T2> : UnityEvent<T0, T1, T2>, IEventListener
    {
        public EventListener() { }
    }
    class EventListener<T0, T1, T2, T3> : UnityEvent<T0, T1, T2, T3>, IEventListener
    {
        public EventListener() { }
    }
    #endregion

    private static EventNotify mInstance;
    public static EventNotify Instance { get { if (mInstance == null) mInstance = new EventNotify(); return mInstance; } }


    private Dictionary<int, Dictionary<Type, IEventListener>> mListeners = new Dictionary<int, Dictionary<Type, IEventListener>>();

    public void AddListener(int id, UnityAction call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic) == false)
        {
            dic = new Dictionary<Type, IEventListener>();
            mListeners.Add(id, dic);
        }
        Type type = typeof(EventListener);
        if (dic.TryGetValue(type, out IEventListener o) == false)
        {
            o = new EventListener();
            dic.Add(type, o);
        }
        EventListener listener = o as EventListener;
        listener.RemoveListener(call);
        listener.AddListener(call);
    }
    public void AddListener<T0>(int id, UnityAction<T0> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic) == false)
        {
            dic = new Dictionary<Type, IEventListener>();
            mListeners.Add(id, dic);
        }
        Type type = typeof(EventListener<T0>);
        if (dic.TryGetValue(type, out IEventListener o) == false)
        {
            o = new EventListener<T0>();
            dic.Add(type, o);
        }
        EventListener<T0> listener = o as EventListener<T0>;
        listener.RemoveListener(call);
        listener.AddListener(call);
    }
    public void AddListener<T0, T1>(int id, UnityAction<T0, T1> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic) == false)
        {
            dic = new Dictionary<Type, IEventListener>();
            mListeners.Add(id, dic);
        }
        Type type = typeof(EventListener<T0, T1>);
        if (dic.TryGetValue(type, out IEventListener o) == false)
        {
            o = new EventListener<T0, T1>();
            dic.Add(type, o);
        }
        EventListener<T0, T1> listener = o as EventListener<T0, T1>;
        listener.RemoveListener(call);
        listener.AddListener(call);
    }
    public void AddListener<T0, T1, T2>(int id, UnityAction<T0, T1, T2> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic) == false)
        {
            dic = new Dictionary<Type, IEventListener>();
            mListeners.Add(id, dic);
        }
        Type type = typeof(EventListener<T0, T1, T2>);
        if (dic.TryGetValue(type, out IEventListener o) == false)
        {
            o = new EventListener<T0, T1, T2>();
            dic.Add(type, o);
        }
        EventListener<T0, T1, T2> listener = o as EventListener<T0, T1, T2>;
        listener.RemoveListener(call);
        listener.AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3>(int id, UnityAction<T0, T1, T2, T3> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic) == false)
        {
            dic = new Dictionary<Type, IEventListener>();
            mListeners.Add(id, dic);
        }
        Type type = typeof(EventListener<T0, T1, T2, T3>);
        if (dic.TryGetValue(type, out IEventListener o) == false)
        {
            o = new EventListener<T0, T1, T2, T3>();
            dic.Add(type, o);
        }
        EventListener<T0, T1, T2, T3> listener = o as EventListener<T0, T1, T2, T3>;
        listener.RemoveListener(call);
        listener.AddListener(call);
    }
    public void RemoveListener(int id, UnityAction call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener listener = o as EventListener;
                listener.RemoveListener(call);
            }
        }
    }
    public void RemoveListener<T0>(int id, UnityAction<T0> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener<T0>);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener<T0> listener = o as EventListener<T0>;
                listener.RemoveListener(call);
            }
        }
    }
    public void RemoveListener<T0, T1>(int id, UnityAction<T0, T1> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener<T0, T1>);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener<T0, T1> listener = o as EventListener<T0, T1>;
                listener.RemoveListener(call);
            }
        }
    }
    public void RemoveListener<T0, T1, T2>(int id, UnityAction<T0, T1, T2> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener<T0, T1, T2>);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener<T0, T1, T2> listener = o as EventListener<T0, T1, T2>;
                listener.RemoveListener(call);
            }
        }
    }
    public void RemoveListener<T0, T1, T2, T3>(int id, UnityAction<T0, T1, T2, T3> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener<T0, T1, T2, T3>);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener<T0, T1, T2, T3> listener = o as EventListener<T0, T1, T2, T3>;
                listener.RemoveListener(call);
            }
        }
    }

    public void Invoke(int id)
    {
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener listener = o as EventListener;
                listener.Invoke();
            }
        }
    }
    public void Invoke<T0>(int id, T0 arg0)
    {
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener<T0>);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener<T0> listener = o as EventListener<T0>;
                listener.Invoke(arg0);
            }
        }
    }
    public void Invoke<T0, T1>(int id, T0 arg0, T1 arg1)
    {
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener<T0, T1>);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener<T0, T1> listener = o as EventListener<T0, T1>;
                listener.Invoke(arg0, arg1);
            }
        }
    }
    public void Invoke<T0, T1, T2>(int id, T0 arg0, T1 arg1, T2 arg2)
    {
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener<T0, T1, T2>);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener<T0, T1, T2> listener = o as EventListener<T0, T1, T2>;
                listener.Invoke(arg0, arg1, arg2);
            }
        }
    }
    public void Invoke<T0, T1, T2, T3>(int id, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            Type type = typeof(EventListener<T0, T1, T2, T3>);
            if (dic.TryGetValue(type, out IEventListener o))
            {
                EventListener<T0, T1, T2, T3> listener = o as EventListener<T0, T1, T2, T3>;
                listener.Invoke(arg0, arg1, arg2, arg3);
            }
        }
    }

    public void Clear(int id)
    {
        if (mListeners.TryGetValue(id, out Dictionary<Type, IEventListener> dic))
        {
            dic.Clear();
        }
        mListeners.Remove(id);
    }
    public void Clear()
    {
        mListeners.Clear();
    }
}
