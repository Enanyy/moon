using System;
using UnityEngine;
public abstract class Singleton<T> where T: class,new ()
{
    private static T mInstance;
    public static T Instance
    {
        get
        {
            if (mInstance== null)
            {
                mInstance = new T();
            }
            return mInstance;
        }
    }

    public static void Destroy()
    {
        mInstance = null;
    }
}

public abstract class MonoSingleton<T> :MonoBehaviour where T:MonoBehaviour
{
    private static T mInstance;
    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                var objects = FindObjectsOfType<T>();
                for (int i = 0; i < objects.Length; ++i)
                {
                    if (i == 0)
                    {
                        mInstance = objects[i];
                    }
                    else
                    {
                        Destroy(objects[i].gameObject);
                    }
                }
                if (mInstance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(go);
                    mInstance = go.AddComponent<T>();
                }
            }
            return mInstance;
        }
    }
    public static void Destroy()
    {
        if(mInstance)
        {
            Destroy(mInstance.gameObject);
        }
        mInstance = null;
    }

  
}

