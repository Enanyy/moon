using System.Collections.Generic;
using UnityEngine.Events;
using System;
#region Listener
public interface IEventListener
{
    void Clear();
}
public class EventListener : IEventListener
{
    public List<Action> listeners = new List<Action>();

    public void AddListener(Action action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action action)
    {
        listeners.Remove(action);
    }

    public void Invoke()
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke();
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}
public class EventListener<T0> : IEventListener
{
    public List<Action<T0>> listeners = new List<Action<T0>>();

    public void AddListener(Action<T0> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action<T0> action)
    {
        listeners.Remove(action);
    }

    public void Invoke(T0 param)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke(param);
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}
public class EventListener<T0, T1> : IEventListener
{
    public List<Action<T0, T1>> listeners = new List<Action<T0, T1>>();

    public void AddListener(Action<T0, T1> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action<T0, T1> action)
    {
        listeners.Remove(action);
    }

    public void Invoke(T0 t0, T1 t1)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke(t0, t1);
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}
public class EventListener<T0, T1, T2> : IEventListener
{
    public List<Action<T0, T1, T2>> listeners = new List<Action<T0, T1, T2>>();

    public void AddListener(Action<T0, T1, T2> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action<T0, T1, T2> action)
    {
        listeners.Remove(action);
    }

    public void Invoke(T0 t0, T1 t1, T2 t2)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke(t0, t1, t2);
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}
public class EventListener<T0, T1, T2, T3> : IEventListener
{
    public List<Action<T0, T1, T2, T3>> listeners = new List<Action<T0, T1, T2, T3>>();

    public void AddListener(Action<T0, T1, T2, T3> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action<T0, T1, T2, T3> action)
    {
        listeners.Remove(action);
    }

    public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke(t0, t1, t2, t3);
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}

public class EventListener<T0, T1, T2, T3, T4> : IEventListener
{
    public List<Action<T0, T1, T2, T3, T4>> listeners = new List<Action<T0, T1, T2, T3, T4>>();

    public void AddListener(Action<T0, T1, T2, T3, T4> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action<T0, T1, T2, T3, T4> action)
    {
        listeners.Remove(action);
    }

    public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke(t0, t1, t2, t3, t4);
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}
public class EventListener<T0, T1, T2, T3, T4, T5> : IEventListener
{
    public List<Action<T0, T1, T2, T3, T4, T5>> listeners = new List<Action<T0, T1, T2, T3, T4, T5>>();

    public void AddListener(Action<T0, T1, T2, T3, T4, T5> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action<T0, T1, T2, T3, T4, T5> action)
    {
        listeners.Remove(action);
    }

    public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke(t0, t1, t2, t3, t4, t5);
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}
public class EventListener<T0, T1, T2, T3, T4, T5, T6> : IEventListener
{
    public List<Action<T0, T1, T2, T3, T4, T5, T6>> listeners = new List<Action<T0, T1, T2, T3, T4, T5, T6>>();

    public void AddListener(Action<T0, T1, T2, T3, T4, T5, T6> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action<T0, T1, T2, T3, T4, T5, T6> action)
    {
        listeners.Remove(action);
    }

    public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke(t0, t1, t2, t3, t4, t5, t6);
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}

public class EventListener<T0, T1, T2, T3, T4, T5, T6, T7> : IEventListener
{
    public List<Action<T0, T1, T2, T3, T4, T5, T6, T7>> listeners = new List<Action<T0, T1, T2, T3, T4, T5, T6, T7>>();

    public void AddListener(Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
    {
        listeners.Remove(action);
    }

    public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            listeners[i].Invoke(t0, t1, t2, t3, t4, t5, t6, t7);
        }
    }
    public void Clear()
    {
        listeners.Clear();
    }

}
#endregion
/// <summary>
/// 事件派发器
/// </summary>
public class EventSystem : Singleton<EventSystem>
{
    private Dictionary<int, IEventListener> mListeners = new Dictionary<int, IEventListener>();

    public void AddListener(int id, Action call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener();
            mListeners.Add(id, listener);
        }

        (listener as EventListener).AddListener(call);
    }
    public void AddListener<T0>(int id, Action<T0> call)
    {
        if (call == null)
        {
            return;
        }

        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener<T0>();
            mListeners.Add(id, listener);
        }
        (listener as EventListener<T0>).AddListener(call);

    }
    public void AddListener<T0, T1>(int id, Action<T0, T1> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1>();
            mListeners.Add(id, listener);
        }
        (listener as EventListener<T0, T1>).AddListener(call);
    }
    public void AddListener<T0, T1, T2>(int id, Action<T0, T1, T2> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2>();
            mListeners.Add(id, listener);
        }
        (listener as EventListener<T0, T1, T2>).AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3>(int id, Action<T0, T1, T2, T3> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3>();
            mListeners.Add(id, listener);
        }
        (listener as EventListener<T0, T1, T2, T3>).AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3, T4>(int id, Action<T0, T1, T2, T3, T4> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3, T4>();
            mListeners.Add(id, listener);
        }
        (listener as EventListener<T0, T1, T2, T3, T4>).AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3, T4, T5>(int id, Action<T0, T1, T2, T3, T4, T5> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3, T4, T5>();
            mListeners.Add(id, listener);
        }
        (listener as EventListener<T0, T1, T2, T3, T4, T5>).AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3, T4, T5, T6>(int id, Action<T0, T1, T2, T3, T4, T5, T6> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3, T4, T5, T6>();
            mListeners.Add(id, listener);
        }
        (listener as EventListener<T0, T1, T2, T3, T4, T5, T6>).AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3, T4, T5, T6, T7>(int id, Action<T0, T1, T2, T3, T4, T5, T6, T7> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3, T4, T5, T6, T7>();
            mListeners.Add(id, listener);
        }
        (listener as EventListener<T0, T1, T2, T3, T4, T5, T6, T7>).AddListener(call);
    }

    public void RemoveListener(int id, Action call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener).RemoveListener(call);
        }
    }
    public void RemoveListener<T0>(int id, Action<T0> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0>).RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1>(int id, Action<T0, T1> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1>).RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2>(int id, Action<T0, T1, T2> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2>).RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3>(int id, Action<T0, T1, T2, T3> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3>).RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3, T4>(int id, Action<T0, T1, T2, T3, T4> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3, T4>).RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3, T4, T5>(int id, Action<T0, T1, T2, T3, T4, T5> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3, T4, T5>).RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3, T4, T5, T6>(int id, Action<T0, T1, T2, T3, T4, T5, T6> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3, T4, T5, T6>).RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3, T4, T5, T6, T7>(int id, Action<T0, T1, T2, T3, T4, T5, T6, T7> call)
    {
        if (call == null)
        {
            return;
        }
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3, T4, T5, T6, T7>).RemoveListener(call);
        }
    }
    public void Invoke(int id)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener).Invoke();
        }
    }
    public void Invoke<T0>(int id, T0 arg0)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0>).Invoke(arg0);
        }
    }
    public void Invoke<T0, T1>(int id, T0 arg0, T1 arg1)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1>).Invoke(arg0, arg1);
        }
    }
    public void Invoke<T0, T1, T2>(int id, T0 arg0, T1 arg1, T2 arg2)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2>).Invoke(arg0, arg1, arg2);
        }
    }
    public void Invoke<T0, T1, T2, T3>(int id, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3>).Invoke(arg0, arg1, arg2, arg3);
        }
    }
    public void Invoke<T0, T1, T2, T3, T4>(int id, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3, T4>).Invoke(arg0, arg1, arg2, arg3, arg4);
        }
    }
    public void Invoke<T0, T1, T2, T3, T4, T5>(int id, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3, T4, T5>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
        }
    }
    public void Invoke<T0, T1, T2, T3, T4, T5, T6>(int id, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3, T4, T5, T6>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }
    public void Invoke<T0, T1, T2, T3, T4, T5, T6, T7>(int id, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            (listener as EventListener<T0, T1, T2, T3, T4, T5, T6, T7>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }

    public void Clear(int id)
    {
        if (mListeners.TryGetValue(id, out IEventListener listener))
        {
            listener.Clear();
        }
        mListeners.Remove(id);
    }
    public void Clear()
    {
        mListeners.Clear();
    }
}
