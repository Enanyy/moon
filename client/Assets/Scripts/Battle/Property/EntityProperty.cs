using System;

public interface IEntityProperty
{
    void Clear();
}

public class EntityProperty<T>:IEntityProperty where T:IEquatable<T>
{
    private T mValue;

    public T value
    {
        get { return mValue; }
        set
        {
            T from = mValue;
            mValue = value;
            if (from.Equals(mValue) ==false)
            {
                OnValueChanged(from, mValue);
            }
        }
    }

    public T defaultValue;

    public  EntityProperty(T value, T defaultValue)
    {
        this.value = value;
        this.defaultValue = defaultValue;
    }

    protected virtual void OnValueChanged(T from, T to)
    {

    }

    public void Clear()
    {
        value = defaultValue;
    }
}

