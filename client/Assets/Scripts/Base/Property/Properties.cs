
using System;
using System.Collections;
using System.Collections.Generic;

public class Properties 
{
    public Dictionary<uint, IProperty> properties = new Dictionary<uint, IProperty>();

    public void SetProperty<T>(uint id, T value) where T : IEquatable<T>
    {
        if (properties.ContainsKey(id) == false)
        {
            properties.Add(id, new Property<T>(value, value));
        }
        else
        {
            var property = properties[id] as Property<T>;
            property.value = value;
        }
    }
    public void AddPropertyEvent<T>(uint id, Action<T, T> listener) where T : IEquatable<T>
    {
        if (properties.TryGetValue(id, out IProperty entity))
        {
            var property = entity as Property<T>;
            property.onValueChanged += listener;
        }
    }

    public void RemovePropertyEvent<T>(uint id, Action<T, T> listener) where T : IEquatable<T>
    {
        if (properties.TryGetValue(id, out IProperty entity))
        {
            var property = entity as Property<T>;
            property.onValueChanged -= listener;
        }
    }
    public T GetProperty<T>(uint id, T defaultValue = default) where T : IEquatable<T>
    {
        if (properties.TryGetValue(id, out IProperty entity))
        {
            var property = entity as Property<T>;
            return property.value;
        }
        else
        {
            var property = new Property<T>(defaultValue, defaultValue);

            properties.Add(id, property);
        }

        return defaultValue;
    }

    public Property<T> GetProperty<T>(uint id) where T : IEquatable<T>
    {
        if (properties.TryGetValue(id, out IProperty entity))
        {
            var property = entity as Property<T>;
            return property;
        }
        return null;
    }

}
