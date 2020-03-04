using UnityEngine;
using System.Collections.Generic;
using System;

public enum RemindStatus
{
    Off = 0,
    On = 1,
}

public class RemindSystem
{
    static RemindSystem mInstance;
    public static RemindSystem Instance
    {
        get
        {
            if (mInstance == null) mInstance = new RemindSystem();
            return mInstance;
        }
    }

    private Dictionary<int, int> mCountDic = new Dictionary<int, int>();
    private Dictionary<Action<int>, List<int>> mListenerDic = new Dictionary<Action<int>, List<int>>();

    public void RegisterListener(Action<int> listener, int id)
    {
        if (listener == null)
        {
            return;
        }


        if (mListenerDic.ContainsKey(listener) == false)
        {
            mListenerDic.Add(listener, new List<int>());
        }

        var list = mListenerDic[listener];
        if (list.Contains(id) == false)
        {
            list.Add(id);

            int status = GetCount(list);

            listener(status);
        }
    }

    public void UnRegisterListener(Action<int> listener, int id)
    {
        if (listener == null)
        {
            return;
        }

        if (mListenerDic.TryGetValue(listener, out List<int> list))
        {
            list.Remove(id);

            int count = GetCount(list);

            listener(count);

            if (list.Count <= 0)
            {
                mListenerDic.Remove(listener);
            }
        }
    }

    public int GetCount(int id)
    {
        if (mCountDic.TryGetValue(id, out int count))
        {
            return count;
        }
        return 0;
    }
    public int GetCount(List<int> list)
    {
        int count = 0;
        if (list != null)
        {
            for (int i = 0, max = list.Count; i < max; ++i)
            {
                count += GetCount(list[i]);
            }
        }
        return count;
    }


    public void ChangeCount(int id, int count)
    {
        if (mCountDic.ContainsKey(id))
        {
            mCountDic[id] = count;
        }
        else
        {
            mCountDic.Add(id, count);
        }

        var it = mListenerDic.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Value.Contains(id))
            {
                count = GetCount(it.Current.Value);

                it.Current.Key(count);
            }
        }
    }
    public void ChangeStatus(int id, RemindStatus status)
    {
        ChangeCount(id, (int)status);
    }
}
