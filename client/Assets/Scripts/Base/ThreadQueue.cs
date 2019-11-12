using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public  class ThreadQueue :MonoBehaviour  {

    private static ThreadQueue mInstance;
    private static ThreadQueue Instance
    {
        get
        {
            if(mInstance == null)
            {
                mInstance = FindObjectOfType<ThreadQueue>();
               
            }
            if (mInstance == null)
            {
                GameObject go = new GameObject(typeof(ThreadQueue).Name);
                mInstance = go.AddComponent<ThreadQueue>();
                DontDestroyOnLoad(go);
            }
            return mInstance;
        }
    }

	List<ThreadBase> threads = new List<ThreadBase>();
    public static int maxThreads = 8;
    static int numThreads;
    public static void RunAsync<T>(Func<T> func, Action<T> callback = null,float delay = 0)
    {
        AddThread(new ThreadFunc<T>(func,callback,delay));
	}

    public static void RunAsync(Action func, Action callback= null, float delay = 0)
    {
        AddThread(new ThreadAction(func, callback, delay));
    }

    private static void AddThread(ThreadBase thread)
    {
        //可以立即执行的时候尝试去执行，但不一定能立即执行成功
        if(thread.isExecuteable)
        {
            thread.Execute();
        }
        Instance.threads.Add(thread);
    }

    private void Update()
    {
        for(int i = 0; i < threads.Count;)
        {
            var t = threads[i];
            if (t == null)
            {
                threads.RemoveAt(i);

                continue;
            }
            else if (t.isCompleted)
            {
                if (t.exception == null)
                {
                    t.OnCompleted();
                }
                else
                {
                    Debug.LogError(t.exception);
                }

                threads.RemoveAt(i);

                continue;
            }
            else if (t.isExecuted == false)
            {
                if (t.isExecuteable)
                {
                    t.Execute();
                }
            }

       
            i++;
        }
    }

    abstract class ThreadBase
    {
        public Exception exception { get; protected set; }
        public bool isExecuted { get; protected set; }
        public bool isCompleted { get; protected set; }

        public  float delay { get; protected set; }
        private float mBeginTime;
        public ThreadBase()
        {
            mBeginTime = Time.time;
            delay = 0;
        }

        public virtual bool isExecuteable { get { return Time.time >= mBeginTime + delay; } }
        public void Execute()
        {
            if (numThreads < maxThreads)
            {
                ///加入到线程池
                isExecuted = ThreadPool.QueueUserWorkItem(TryExecute);
                if (isExecuted)
                {
                    //线程数量加1
                    Interlocked.Increment(ref numThreads);
                }
            }
        }
        private void TryExecute(object obj)
        {
            try
            {
                OnExecute();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                //线程数量减1
                Interlocked.Decrement(ref numThreads);

                isCompleted = true;
            }
        }
        protected abstract void OnExecute();

        public virtual void OnCompleted() {}

    }
    class ThreadAction : ThreadBase
    {
        
        public readonly Action callback;
        public readonly Action func;
     
        public ThreadAction(Action func, Action callback,float delay = 0)
        {
            this.callback = callback;
            this.func = func;
            this.delay = delay;
            isCompleted = false;
            isExecuted = false;
        }

        protected override void OnExecute()
        {
            if(func!= null)
            {
                func();
            }
        }


        public override void OnCompleted()
        {
            if(callback!=null)
            {
                callback();
            }
        }

    }
   
    class ThreadFunc<T>:ThreadBase
    {
      
        public readonly Action<T> callback;
		public T data { get; private set; }
        public readonly Func<T> func;

      
        public ThreadFunc (Func<T> func, Action<T> callback, float delay = 0)
		{
			this.callback = callback;
            this.func = func;
            this.delay = delay;
        }
        protected override void OnExecute()
        {
            if (func != null)
            {
                data = func();
            }
        }
        

        public override void OnCompleted()
        {
            if(callback!= null)
            {
                callback(data);
            }
        }
	}
}
