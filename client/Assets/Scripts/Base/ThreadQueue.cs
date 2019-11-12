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

	List<IThread> threads = new List<IThread>();
    public static int maxThreads = 8;
    static int numThreads;
    public static void RunAsync<T>(Func<T> func, Action<T> callback = null,float delay = 0)
    {
        Instance.threads.Add(new ThreadFunc<T>(func,callback,delay));
	}

    public static void RunAsync(Action func, Action callback= null, float delay = 0)
    {
        Instance.threads.Add(new ThreadAction(func, callback, delay));
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

    private static bool RunAsync(Action action)
    {
        if(numThreads>= maxThreads)
        {
            return false;
        }
        else
        {
            Interlocked.Increment(ref numThreads);

            new Thread(delegate () 
            {
                try
                {
                    if (action != null)
                    {
                        action();
                    }
                }
                catch
                {

                }
                finally
                {
                    Interlocked.Decrement(ref numThreads);
                }

            }).Start();

            return true;
        }

    }



    interface IThread
    {
        Exception exception { get; }
        bool isExecuted { get; }
        bool isCompleted { get; }
        bool isExecuteable { get; }
        void Execute();
        void OnCompleted();

    }
    class ThreadAction : IThread
    {
        public Exception exception { get; private set; }
        public readonly Action callback;
        public readonly Action func;
        public bool isExecuted { get; private set; }
        public bool isCompleted { get; private set; }

        private float mExecuteTime;
        public bool isExecuteable
        {
            get { return Time.time >= mExecuteTime; }
        }
        public ThreadAction(Action func, Action callback,float delay = 0)
        {
            this.callback = callback;
            this.func = func;
            isCompleted = false;
            isExecuted = false;
            mExecuteTime = Time.time + delay ;
        }

        public void Execute()
        {
            isExecuted = RunAsync(TryExecute);
        }
        private void TryExecute()
        {
            try
            {
                if (func != null)
                {
                    func();
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                isCompleted = true;
            }
        }


        public void OnCompleted()
        {
            if(callback!=null)
            {
                callback();
            }
        }

    }
   
    class ThreadFunc<T>:IThread
    {
        public Exception exception { get; private set; }

        public readonly Action<T> callback;
		public T data { get; private set; }
        public readonly Func<T> func;

        public bool isExecuted { get; private set; }
        public bool isCompleted { get; private set; }

        private float mExecuteTime;
        public bool isExecuteable
        {
            get { return Time.time >= mExecuteTime; }
        }
        public ThreadFunc (Func<T> func, Action<T> callback, float delay = 0)
		{
			this.callback = callback;
            this.func = func;
            mExecuteTime = Time.time + delay;
        }
        public void Execute()
        {
            isExecuted = RunAsync(TryExecute);
        }
        private void TryExecute()
        {
            try
            {
                if (func != null)
                {
                    data = func();
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                isCompleted = true;
            }
        }

        public void OnCompleted()
        {
            if(callback!= null)
            {
                callback(data);
            }
        }
	}
}
