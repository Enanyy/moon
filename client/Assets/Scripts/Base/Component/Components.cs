using System;
using System.Collections.Generic;
public abstract class Components<C> where C : Components<C>
{
    public List<IComponent<C>> components { get; } = new List<IComponent<C>>();

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
            components.Add(component);
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
        for (int i = 0; i < components.Count; ++i)
        {
            if (components[i].GetType() == type)
            {
                return components[i];
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
        for (int i = components.Count - 1; i >= 0; --i)
        {
            if (components[i].GetType() == type)
            {
                components[i].OnDestroy();
                components.RemoveAt(i);
            }
        }
    }

    public void RemoveComponent(IComponent<C> component)
    {
        for (int i = components.Count - 1; i >= 0; --i)
        {
            if (components[i] == component)
            {
                components.RemoveAt(i);
            }
        }
    }

    public virtual void OnUpdate(float deltaTime)
    {
        for (int i = 0; i < components.Count; ++i)
        {
            components[i].OnUpdate(deltaTime);
        }
    }
    public virtual void Destroy()
    {
        for(int i = 0; i < components.Count;++i)
        {
            components[i].OnDestroy();
        }
        components.Clear();
    }

    public virtual void Clear()
    {
        components.Clear();
    }
}

