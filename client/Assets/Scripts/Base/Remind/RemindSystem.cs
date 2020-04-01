using UnityEngine;
using System.Collections.Generic;
using System;

public enum RemindStatus
{
    Off = 0,
    On = 1,
}
public class Reminder
{
    private List<int> mRemindList = new List<int>();
    private Action<int> mCallback;

    public Reminder(Action<int> call)
    {
        mCallback = call;
    }
    public Reminder(Action<int> call, params int[] id)
    {
        mCallback = call;
        mRemindList.AddRange(id);
    }

    public void AddRemindID(int id)
    {
        if(mRemindList.Contains(id)==false)
        {
            mRemindList.Add(id);
        }
    }
    public void RemoveRemindID(int id)
    {
        if(mRemindList.Remove(id))
        {
        }
    }

    public void CheckStatus()
    {
        int count = RemindSystem.Instance.GetCount(mRemindList);

        if(mCallback!=null)
        {
            mCallback(count);
        }
    }

}
public class RemindSystem:Singleton<RemindSystem>
{

    private Dictionary<int, int> mCountDic = new Dictionary<int, int>();
    
    private List<Reminder> mReminderList = new List<Reminder>();
    public void RegisterListener(Reminder reminder)
    {
        if (reminder == null)
        {
            return;
        }
        if(mReminderList.Contains(reminder))
        {
            mReminderList.Add(reminder);

            reminder.CheckStatus();
        }     
    }

    public void UnRegisterListener(Reminder reminder)
    {
        if (reminder == null)
        {
            return;
        }

        mReminderList.Remove(reminder);
        reminder.CheckStatus();
        
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

        for (int i = mReminderList.Count -1; i >= 0; --i)
        {
            mReminderList[i].CheckStatus();
        }
    }
    public void ChangeStatus(int id, RemindStatus status)
    {
        ChangeCount(id, (int)status);
    }
}
