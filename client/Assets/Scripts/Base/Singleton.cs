using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISingleton
{
    void OnDestroy();
}
public static class Singleton
{
    readonly static List<ISingleton> instances = new List<ISingleton>();
    public static void AddInstance(ISingleton instance)
    {
        if(instance== null)
        {
            return;
        }
        if(instances.Contains(instance)==false)
        {
            instances.Add(instance);
        }
    }
    public static void RemoveInstance(ISingleton instance)
    {
        if(instance != null)
        {
            instances.Remove(instance);
        }
    }

    public static void ClearAll()
    {
        for(int i = 0; i < instances.Count;++i)
        {
            var behaviour = instances[i] as MonoBehaviour;
            if(behaviour!=null)
            {
                UnityEngine.Object.Destroy(behaviour.gameObject);
            }
            else
            {
                instances[i].OnDestroy();
            }
        }
        instances.Clear();
    }
}

public abstract class Singleton<T> : ISingleton where T : class, ISingleton, new()
{
    private static T mInstance;
    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new T();
                Singleton.AddInstance(mInstance);
            }
            return mInstance;
        }
    }
    public virtual void OnDestroy()
    {

    }

    public static void Destroy()
    {
        Singleton.RemoveInstance(mInstance);
        if (mInstance != null)
        {
            mInstance.OnDestroy();
        }
        mInstance = null;
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoBehaviour, ISingleton
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
                Singleton.AddInstance(mInstance);
            }
            return mInstance;
        }
    }
    public virtual void OnDestroy()
    {

    }

    public static void Destroy()
    {
        if (mInstance)
        {
            Singleton.RemoveInstance(mInstance);
            Destroy(mInstance.gameObject);
        }
        mInstance = null;
    }
}

