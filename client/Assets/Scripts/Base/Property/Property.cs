using System;

public interface IProperty
{
    void Clear();
}

public class Property<T>:IProperty where T:IEquatable<T>
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

    public event Action<T,T> onValueChanged;

    public Property(T value, T defaultValue)
    {
        this.value = value;
        this.defaultValue = defaultValue;
    }

    protected virtual void OnValueChanged(T from, T to)
    {
        if(onValueChanged!=null)
        {
            onValueChanged(from, to);
        }
    }

    public void Clear()
    {
        value = defaultValue;
    }
}

