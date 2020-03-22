using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DelayManager : MonoSingleton<DelayManager>
{
    #region Inner Class
    class DelayTask : IPoolObject
    {
        private float mDelay;
        private Action mCallback;
        public void Init(float delay, Action callback)
        {
            mDelay = delay;

            mCallback = callback;

        }

        public void OnConstruct()
        {
            Clear();
        }

        public void OnDestruct()
        {
            Clear();
        }


        public bool DoTask(float deltaTime)
        {
            if (mDelay > 0)
            {
                mDelay -= deltaTime;
                if (mDelay <= 0)
                {
                    if (mCallback != null)
                    {
                        mCallback();
                    }
                }
            }
            return mDelay <= 0;
        }

        public void Clear()
        {
            mDelay = 0;
            mCallback = null;
        }
    }
    #endregion
 
    private List<DelayTask> mDelayTasks = new List<DelayTask>();

    // Update is called once per frame
    void Update()
    {
        for (int i = mDelayTasks.Count - 1; i >= 0; --i)
        {
            if (mDelayTasks[i] == null || mDelayTasks[i].DoTask(Time.deltaTime))
            {
                if (mDelayTasks[i] != null)
                {
                    ObjectPool.ReturnInstance(mDelayTasks[i]);
                }
                mDelayTasks.RemoveAt(i);
            }
        }
    }

    public void AddDelayTask(float delay, Action callback)
    {
        if (callback == null)
        {
            return;
        }
        var delayTask = ObjectPool.GetInstance<DelayTask>();
        delayTask.Init(delay, callback);
        mDelayTasks.Add(delayTask);
    }


    public void Clear()
    {
        for (int i = 0; i < mDelayTasks.Count; ++i)
        {
            if (mDelayTasks[i] != null)
            {
                ObjectPool.ReturnInstance(mDelayTasks[i]);
            }
        }
        mDelayTasks.Clear();
    }
}
