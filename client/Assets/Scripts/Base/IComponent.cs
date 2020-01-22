using System;
using System.Collections.Generic;

public interface IComponent
{
    IComponent Parent { get; set; }

    Dictionary<Type,IComponent > Components { get; set; }
    void OnComponentInitialize();
    void OnComponentDestroy();
    
}


public static class ComponentUtility
{
    public static IComponent GetRoot(this IComponent component)
    {
        if(component == null)
        {
            return default;
        }
        if(component.Parent == null)
        {
            return component;
        }
        else
        {
            return component.Parent.GetRoot();
        }
    }
    public static T AddComponent<T>(this IComponent parent) where T : IComponent, new()
    {
        if (parent == null)
        {
            return default;
        }
        if (parent.Components == null)
        {
            parent.Components = new Dictionary<Type, IComponent>();
        }


        var type = typeof(T);

        var component = parent.GetComponent<T>();
        if (component == null)
        {
            component = new T();
            component.Parent = parent;
            component.OnComponentInitialize();
            parent.Components.Add(type, component);
        }

        return component;

    }

    public static void RemoveComponent<T>(this IComponent parent) where T : IComponent
    {
        if (parent == null || parent.Components == null)
        {
            return;
        }

        var type = typeof(T);

        if (parent.Components.TryGetValue(type, out IComponent value))
        {
            value.OnComponentDestroy();
            parent.Components.Remove(type);
        }
        else
        {
            var it = parent.Components.GetEnumerator();
            while(it.MoveNext())
            {
                it.Current.Value.RemoveComponent<T>();
            }
        }
    }

    public static T GetComponent<T>(this IComponent parent) where T : IComponent
    {
        if (parent == null || parent.Components == null)
        {
            return default;
        }
        var type = typeof(T);
        if (parent.GetType() == type)
        {
            return (T)parent;
        }
        if (parent.Components.TryGetValue(type, out IComponent value))
        {
            return (T)value;
        }
        else
        {
            var it = parent.Components.GetEnumerator();
            while (it.MoveNext())
            {
                var component = it.Current.Value.GetComponent<T>();
                if (component != null)
                {
                    return (T)component;
                }

            }
        }
        return default;
    }

    public static void Foreach(this IComponent component,Action<IComponent> callback)
    {
        if(component == null || callback == null)
        {
            return;
        }

        callback(component);

        if(component.Components != null)
        {
            var it = component.Components.GetEnumerator();
            while(it.MoveNext())
            {
                it.Current.Value.Foreach(callback);
            }
        }
    }

    public static void Foreach<T>(this IComponent component, Action<T> callback) where T : IComponent
    {
        if (component == null || callback == null)
        {
            return;
        }
        var type = typeof(T);
        if (component.GetType() == type)
        {
            callback((T)component);
        }


        if (component.Components != null)
        {
            var it = component.Components.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.Value.Foreach(callback);
            }
        }

    }
}
