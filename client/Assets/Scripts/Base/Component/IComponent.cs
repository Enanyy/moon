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
        if (component == null)
        {
            return default;
        }
        if (component.Parent == null)
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

        if (parent.TryGetComponent(out T component) == false)
        {
            component = new T();
            component.Parent = parent;
            parent.Components.Add(typeof(T), component);
            component.OnComponentInitialize();
        }

        return component;

    }

    public static bool AddComponent(this IComponent parent, IComponent component)
    {
        if (parent == null || component == null)
        {
            return false;
        }

        if (parent.Components == null)
        {
            parent.Components = new Dictionary<Type, IComponent>();
        }
        var type = component.GetType();
        if (parent.Components.ContainsKey(type) == false)
        {
            component.Parent = parent;

            parent.Components.Add(type, component);
            component.OnComponentInitialize();

            return true;
        }

        return false;
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
            value.RemoveAllComponents();
            parent.Components.Remove(type);
        }
        else
        {
            var it = parent.Components.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.Value.RemoveComponent<T>();
            }
        }
    }

    public static void RemoveComponent(this IComponent parent, IComponent component)
    {
        if (parent == null || parent.Components == null || component == null)
        {
            return;
        }

        var type = component.GetType();
        if (parent.Components.TryGetValue(type, out IComponent value))
        {
            if (value == component)
            {
                value.OnComponentDestroy();
                value.RemoveAllComponents();
                parent.Components.Remove(type);
            }
        }
        else
        {
            var it = parent.Components.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.Value.RemoveComponent(component);
            }
        }
    }

    public static void RemoveAllComponents(this IComponent component)
    {
        if (component == null || component.Components == null)
        {
            return;
        }

        var it = component.Components.GetEnumerator();
        while (it.MoveNext())
        {
            var value = it.Current.Value;
            value.OnComponentDestroy();
            value.RemoveAllComponents();
        }
        component.Components.Clear();
    }

    public static void RemoveFromParent(this IComponent component)
    {
        if (component == null)
        {
            return;
        }
        if (component.Parent == null)
        {
            if (component.Components != null)
            {
                var it = component.Components.GetEnumerator();
                while (it.MoveNext())
                {
                    var value = it.Current.Value;
                    value.OnComponentDestroy();
                    value.RemoveAllComponents();
                }
                component.Components.Clear();
            }
            component.OnComponentDestroy();
        }
        else
        {
            component.Parent.RemoveComponent(component);
        }
    }

    public static bool TryGetComponent<T>(this IComponent parent, out T component) where T : IComponent
    {
        component = default;
        if (parent == null || parent.Components == null)
        {
            return false;
        }
        var type = typeof(T);
        if (parent.GetType() == type)
        {
            component = (T)parent;
            return true;
        }
        if (parent.Components.TryGetValue(type, out IComponent value))
        {
            component = (T)value;
            return true;
        }
        else
        {
            var it = parent.Components.GetEnumerator();
            while (it.MoveNext())
            {
                var result = it.Current.Value.TryGetComponent(out component);
                if (result)
                {
                    return result;
                }
            }
        }
        return false;
    }

    public static void Foreach(this IComponent component, Action<IComponent> callback)
    {
        if (component == null || component.Components == null || callback == null)
        {
            return;
        }

        var it = component.Components.GetEnumerator();
        while (it.MoveNext())
        {
            callback(it.Current.Value);

            it.Current.Value.Foreach(callback);
        }

    }

    public static void Foreach<T>(this IComponent component, Action<T> callback) where T : IComponent
    {
        if (component == null || component.Components == null || callback == null)
        {
            return;
        }
        var type = typeof(T);


        var it = component.Components.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Value.GetType() == type)
            {
                callback((T)it.Current.Value);
            }
            it.Current.Value.Foreach(callback);
        }
    }
}
