using System.Collections.Generic;
using UnityEngine.Events;
using System;
#region Listener
public interface IEventListener
{
    void AddListener(Delegate action);
    void RemoveListener(Delegate action);
    void Clear();
}
public class EventListener : IEventListener
{
    public List<Action> listeners = new List<Action>();

    public void AddListener(Delegate action)
    {
        AddListener((Action)action);
    }
    public void AddListener(Action action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action)action);
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

    public void AddListener(Delegate action)
    {
        AddListener((Action<T0>)action);
    }
    public void AddListener(Action<T0> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action<T0>)action);
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

    public void AddListener(Delegate action)
    {
        AddListener((Action<T0,T1>)action);
    }
    public void AddListener(Action<T0, T1> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action<T0,T1>)action);
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

    public void AddListener(Delegate action)
    {
        AddListener((Action<T0, T1,T2>)action);
    }
    public void AddListener(Action<T0, T1, T2> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action<T0, T1,T2>)action);
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
    public void AddListener(Delegate action)
    {
        AddListener((Action<T0, T1,T2,T3>)action);
    }
    public void AddListener(Action<T0, T1, T2, T3> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action<T0, T1, T2,T3>)action);
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

    public void AddListener(Delegate action)
    {
        AddListener((Action<T0, T1, T2, T3,T4>)action);
    }
    public void AddListener(Action<T0, T1, T2, T3, T4> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action<T0, T1, T2, T3,T4>)action);
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
    public void AddListener(Delegate action)
    {
        AddListener((Action<T0, T1, T2, T3, T4,T5>)action);
    }
    public void AddListener(Action<T0, T1, T2, T3, T4, T5> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action<T0, T1, T2, T3, T4,T5>)action);
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

    public void AddListener(Delegate action)
    {
        AddListener((Action<T0, T1, T2, T3, T4, T5,T6>)action);
    }
    public void AddListener(Action<T0, T1, T2, T3, T4, T5, T6> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action<T0, T1, T2, T3, T4, T5,T6>)action);
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

    public void AddListener(Delegate action)
    {
        AddListener((Action<T0, T1, T2, T3, T4, T5, T6,T7>)action);
    }
    public void AddListener(Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
    {
        if (action != null && listeners.Contains(action) == false)
        {
            listeners.Add(action);
        }
    }
    public void RemoveListener(Delegate action)
    {
        RemoveListener((Action<T0, T1, T2, T3, T4, T5, T6,T7>)action);
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
    private struct EventIndex : IEquatable<EventIndex>
    {
        public uint type;
        public uint id;

        public EventIndex(uint eventType, uint eventId)
        {
            type = eventType;
            id = eventId;
        }

        public bool Equals(EventIndex other)
        {
            return other.type == type && other.id == id;
        }
        public static bool operator ==(EventIndex l, EventIndex r)
        {
            return l.Equals(r);
        }
        public static bool operator !=(EventIndex l, EventIndex r)
        {
            return !l.Equals(r);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return Equals((EventIndex)obj);
        }
        public override int GetHashCode()
        {
            return (type ^ id).GetHashCode();
        }
    }
    private struct EventID : IEquatable<EventID>
    {
        public EventIndex index;
        public uint target;


        public EventID(uint eventType, uint eventId, uint eventTarget)
        {
            index = new EventIndex(eventType, eventId);
            target = eventTarget;
        }
        public EventID(EventIndex eventIndex, uint eventTarget)
        {
            index = eventIndex;
            target = eventTarget;
        }

        public bool Equals(EventID other)
        {
            return other.index.Equals(index) && other.target == target;
        }
        public static bool operator ==(EventID l, EventID r)
        {
            return l.Equals(r);
        }
        public static bool operator !=(EventID l, EventID r)
        {
            return !l.Equals(r);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return Equals((EventID)obj);
        }
        public override int GetHashCode()
        {
            return (index.GetHashCode()^target).GetHashCode();
        }

    }

    private Dictionary<EventIndex, List<EventID>> mEventID = new Dictionary<EventIndex, List<EventID>>();
    private Dictionary<EventID, IEventListener> mListeners = new Dictionary<EventID, IEventListener>();

    public void AddListener(uint type, uint id, uint target, Action call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener();
            mListeners.Add(eventID, listener);
        }

        listener.AddListener(call);
    }
    public void AddListener<T0>(uint type, uint id, uint target, Action<T0> call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener<T0>();
            mListeners.Add(eventID, listener);
        }

        listener.AddListener(call);

    }
    public void AddListener<T0, T1>(uint type, uint id, uint target, Action<T0, T1> call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1>();
            mListeners.Add(eventID, listener);
        }
        listener.AddListener(call);
    }
    public void AddListener<T0, T1, T2>(uint type, uint id, uint target, Action<T0, T1, T2> call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2>();
            mListeners.Add(eventID, listener);
        }
        listener.AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3>(uint type, uint id, uint target, Action<T0, T1, T2, T3> call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3>();
            mListeners.Add(eventID, listener);
        }
        listener.AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3, T4>(uint type, uint id, uint target, Action<T0, T1, T2, T3, T4> call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3, T4>();
            mListeners.Add(eventID, listener);
        }
        listener.AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3, T4, T5>(uint type, uint id, uint target, Action<T0, T1, T2, T3, T4, T5> call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3, T4, T5>();
            mListeners.Add(eventID, listener);
        }
        listener.AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3, T4, T5, T6>(uint type, uint id, uint target, Action<T0, T1, T2, T3, T4, T5, T6> call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3, T4, T5, T6>();
            mListeners.Add(eventID, listener);
        }
        listener.AddListener(call);
    }
    public void AddListener<T0, T1, T2, T3, T4, T5, T6, T7>(uint type, uint id, uint target, Action<T0, T1, T2, T3, T4, T5, T6, T7> call)
    {
        if (call == null)
        {
            return;
        }
        EventIndex index = new EventIndex(type, id);
        EventID eventID = new EventID(index, target);
        if (mEventID.TryGetValue(index, out List<EventID> list) == false)
        {
            list = new List<EventID>();
            mEventID.Add(index, list);
        }
        if (list.Contains(eventID) == false)
        {
            list.Add(eventID);
        }
        if (mListeners.TryGetValue(eventID, out IEventListener listener) == false)
        {
            listener = new EventListener<T0, T1, T2, T3, T4, T5, T6, T7>();
            mListeners.Add(eventID, listener);
        }
        listener.AddListener(call);
    }

    public void RemoveListener(uint type, uint id, uint target, Action call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);

        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void RemoveListener<T0>(uint type, uint id, uint target, Action<T0> call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);

        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1>(uint type, uint id, uint target, Action<T0, T1> call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);

        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2>(uint type, uint id, uint target, Action<T0, T1, T2> call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);

        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3>(uint type, uint id, uint target, Action<T0, T1, T2, T3> call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);

        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3, T4>(uint type, uint id, uint target, Action<T0, T1, T2, T3, T4> call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);

        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3, T4, T5>(uint type, uint id, uint target, Action<T0, T1, T2, T3, T4, T5> call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);

        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3, T4, T5, T6>(uint type, uint id, uint target, Action<T0, T1, T2, T3, T4, T5, T6> call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);

        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void RemoveListener<T0, T1, T2, T3, T4, T5, T6, T7>(uint type, uint id, uint target, Action<T0, T1, T2, T3, T4, T5, T6, T7> call)
    {
        if (call == null)
        {
            return;
        }
        EventID eventID = new EventID(type, id, target);
        if (mListeners.TryGetValue(eventID, out IEventListener listener))
        {
            listener.RemoveListener(call);
        }
    }
    public void Invoke(uint type, uint id, uint target, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener).Invoke();
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener).Invoke();
            }
        }
    }
    public void Invoke<T0>(uint type, uint id, uint target, T0 arg0, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener<T0>).Invoke(arg0);
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener<T0>).Invoke(arg0);
            }
        }
    }
    public void Invoke<T0, T1>(uint type, uint id, uint target, T0 arg0, T1 arg1, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener<T0, T1>).Invoke(arg0, arg1);
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener<T0, T1>).Invoke(arg0, arg1);
            }
        }
    }
    public void Invoke<T0, T1, T2>(uint type, uint id, uint target, T0 arg0, T1 arg1, T2 arg2, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener<T0, T1, T2>).Invoke(arg0, arg1, arg2);
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener<T0, T1, T2>).Invoke(arg0, arg1, arg2);
            }
        }
    }
    public void Invoke<T0, T1, T2, T3>(uint type, uint id, uint target, T0 arg0, T1 arg1, T2 arg2, T3 arg3, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener<T0, T1, T2, T3>).Invoke(arg0, arg1, arg2, arg3);
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener<T0, T1, T2, T3>).Invoke(arg0, arg1, arg2, arg3);
            }
        }
    }
    public void Invoke<T0, T1, T2, T3, T4>(uint type, uint id, uint target, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener<T0, T1, T2, T3, T4>).Invoke(arg0, arg1, arg2, arg3, arg4);
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener<T0, T1, T2, T3, T4>).Invoke(arg0, arg1, arg2, arg3, arg4);
            }
        }
    }
    public void Invoke<T0, T1, T2, T3, T4, T5>(uint type, uint id, uint target, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener<T0, T1, T2, T3, T4, T5>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener<T0, T1, T2, T3, T4, T5>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
            }
        }
    }
    public void Invoke<T0, T1, T2, T3, T4, T5, T6>(uint type, uint id, uint target, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener<T0, T1, T2, T3, T4, T5, T6>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener<T0, T1, T2, T3, T4, T5, T6>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }
    }
    public void Invoke<T0, T1, T2, T3, T4, T5, T6, T7>(uint type, uint id, uint target, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);
        if (all == false)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        (listener as EventListener<T0, T1, T2, T3, T4, T5, T6, T7>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    }
                }
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                (listener as EventListener<T0, T1, T2, T3, T4, T5, T6, T7>).Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
        }
    }

    public void Clear(uint type, uint id, uint target, bool all = false)
    {
        EventIndex index = new EventIndex(type, id);

        if (all)
        {
            if (mEventID.TryGetValue(index, out List<EventID> list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (mListeners.TryGetValue(list[i], out IEventListener listener))
                    {
                        listener.Clear();
                    }
                    mListeners.Remove(list[i]);
                }
                mEventID.Remove(index);
            }
        }
        else
        {
            EventID eventID = new EventID(index, target);
            if (mListeners.TryGetValue(eventID, out IEventListener listener))
            {
                listener.Clear();
            }
            mListeners.Remove(eventID);
        }
    }
    public void Clear()
    {
        mListeners.Clear();
    }
}
