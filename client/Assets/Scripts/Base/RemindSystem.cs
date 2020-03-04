using UnityEngine;
using System.Collections.Generic;
using System;
public enum RemindStatus
{
    Off,
    On,
}

public enum RemindID
{
    None,
}


public class Reminder 
{
    public List<RemindID> IDs = new List<RemindID>();
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

    private Dictionary<int, RemindStatus> mStatusDic = new Dictionary<int, RemindStatus>();
    private Dictionary<int, List<Action<RemindID, RemindStatus>>> mListenerDic = new Dictionary<int, List<Action<RemindID, RemindStatus>>>();

    public void RegisterListener(RemindID id, Action<RemindID,RemindStatus> listener)
    {
        if (listener == null)
        {
            return;
        }

        var remindID = (int)id;

        if (mListenerDic.ContainsKey(remindID) == false)
        {
            mListenerDic.Add(remindID, new List<Action<RemindID, RemindStatus>>());
        }

        var list = mListenerDic[remindID];
        if (list.Contains(listener) == false)
        {
            list.Add(listener);
        }
    }

    public void UnRegisterListener(RemindID id, Action<RemindID, RemindStatus> listener)
    {
        if (listener == null)
        {
            return;
        }
       
        if(mListenerDic.TryGetValue((int)id, out List<Action<RemindID,RemindStatus>> list))
        {
            list.Remove(listener);
        }
    }

    public RemindStatus GetStatus(RemindID id)
    {
        if(mStatusDic.TryGetValue((int)id,out RemindStatus status))
        {
            return status;
        }
        return RemindStatus.Off;
    }

    public void StatusChange(RemindID id, RemindStatus status)
    {
        if (mStatusDic.ContainsKey((int)id))
        {
            mStatusDic[(int)id] = status;
        }
        else
        {
            mStatusDic.Add((int)id, status);
        }   
    }
}
