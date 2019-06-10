using System;

public interface IEntityProperty
{
    void Clear();
}

public class EntityProperty<T>:IEntityProperty
{
    private T mValue;

    public T value
    {
        get { return mValue; }
        set
        {
            T from = mValue;
            mValue = value; 
            OnValueChanged(from, mValue);
        }
    }

    public T defaultValue;

    protected virtual void OnValueChanged(T from, T to)
    {

    }

    public void Clear()
    {
        value = defaultValue;
    }
}

