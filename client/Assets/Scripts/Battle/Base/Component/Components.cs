using System;
using System.Collections.Generic;
public abstract class Components<C> where C : Components<C>
{
    private List<IComponent<C>> mComponentList = new List<IComponent<C>>();
    public List<IComponent<C>> components { get { return mComponentList; } }

    public T AddComponent<T>() where T : class, IComponent<C>, new()
    {
        T component = new T();

        AddComponent(component);

        return component;
    }

    public IComponent<C> AddComponent(IComponent<C> component)
    {
        if (component != null)
        {
            component.agent = this as C;
            mComponentList.Add(component);
            component.OnStart();
        }
        return component;
    }

    public T GetComponent<T>() where T : class, IComponent<C>
    {
        T component = GetComponent(typeof(T)) as T;

        return component;
    }

    public IComponent<C> GetComponent(Type type)
    {
        for (int i = 0; i < mComponentList.Count; ++i)
        {
            if (mComponentList[i].GetType() == type)
            {
                return mComponentList[i];
            }
        }
        return null;
    }

    public void RemoveComponent<T>() where T : class, IComponent<C>
    {
        RemoveComponent(typeof(T));
    }
    public void RemoveComponent(Type type)
    {
        for (int i = mComponentList.Count - 1; i >= 0; --i)
        {
            if (mComponentList[i].GetType() == type)
            {
                mComponentList[i].OnDestroy();
                mComponentList.RemoveAt(i);
            }
        }
    }

    public virtual void OnUpdate(float deltaTime)
    {
        for (int i = 0; i < mComponentList.Count; ++i)
        {
            mComponentList[i].OnUpdate(deltaTime);
        }
    }
    public virtual void Clear()
    {
        for(int i = 0; i < mComponentList.Count;++i)
        {
            mComponentList[i].OnDestroy();
        }
        mComponentList.Clear();
    }
}

