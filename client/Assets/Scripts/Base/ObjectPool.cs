using System;
using System.Collections.Generic;

public interface IPoolObject
{
    /// <summary>
    /// 是否是已经回收的
    /// </summary>
    bool isPool { get; set; }

    void OnCreate();

    void OnReturn();

    //真正销毁的时候调用
    void OnDestroy(); 
}
/// <summary>
/// 对象池,用于保存经常用并可重复用的对象
/// </summary>
public class ObjectPool
{
    public static int maxNum = 8196;

    static private Dictionary<Type, Queue<IPoolObject>> _pool = new Dictionary<Type, Queue<IPoolObject>>();
    /// <summary>
    /// 归还对象到对象池
    /// </summary>
    /// <param name="o">对象</param>
	static public void ReturnInstance<T>(T o, Type type = null) where T : IPoolObject
    {
        if (o == null)
            return;
        if (type == null)
            type = typeof(T);
        o.OnReturn();
        Queue<IPoolObject> l = null;
        if (!_pool.TryGetValue(type, out l))
            l = AddType(type);
        if (l.Count > maxNum)
        {
            UnityEngine.Debug.LogError(string.Format("ReturnInstance maxNum:type={0}", type));
            return;
        }
        if (l.Contains(o))
        {
            UnityEngine.Debug.LogError(string.Format("ReturnInstance Exists:type={0}", type));
            return;
        }
        o.isPool = true;
        l.Enqueue(o);
    }
    static private Queue<IPoolObject> AddType(Type type)
    {
        Queue<IPoolObject> l = new Queue<IPoolObject>();
        _pool.Add(type, l);
        return l;
    }

    /// <summary>
    /// 获取指定类型的对象
    /// </summary>
    /// <param name="service">指定类型</param>
    /// <returns></returns>
	static public T GetInstance<T>(Type type = null) where T : IPoolObject
    {
        if (type == null)
            type = typeof(T);
        Queue<IPoolObject> l = null;
        if (!_pool.TryGetValue(type, out l))
        {
            l = AddType(type);
        }
        int len = l.Count;
        T obj;
        if (len > 0)
        {
            obj = (T)l.Dequeue();
        }
        else
        {
            obj = (T)Activator.CreateInstance(type);// type.Assembly.CreateInstance(type.FullName);
        }
        obj.OnCreate();
        obj.isPool = false;
        return obj;
    }

    /// <summary>
    /// 清理指定类型type的对象池
    /// </summary>
    /// <param name="service">类型</param>
    static public void Clear(Type type,bool includeSubClass = false)
    {
        Queue<IPoolObject> l = null;
        if (_pool.TryGetValue(type, out l))
        {
            var it = l.GetEnumerator();
            while(it.MoveNext())
            {
                IPoolObject o = it.Current as IPoolObject;
                if(o!=null)
                {
                    o.OnDestroy();
                }
            }

            l.Clear();
            _pool.Remove(type);
        }

        if (includeSubClass)
        {
            var it = _pool.GetEnumerator();
            List<Type> list = new List<Type>();
            while (it.MoveNext())
            {
                if(it.Current.Key.IsSubclassOf(type))
                {
                    list.Add(it.Current.Key);

                    var queue = it.Current.Value;
                    while(queue.Count> 0)
                    {
                        IPoolObject o = queue.Dequeue() as IPoolObject;
                        o.OnDestroy();
                    }
                }
            }
            for (int i = 0; i < list.Count; ++i)
            {
                _pool.Remove(list[i]);
            }
        }
    }
    public static void Clear<T>(bool includeSubClass = false)
    {
        Clear(typeof(T), includeSubClass);
    }
    /// <summary>
    /// 清理所有类型的对象池
    /// </summary>
    static public void Clear()
    {
        _pool.Clear();

        var it = _pool.GetEnumerator();
        while (it.MoveNext())
        {
            var queue = it.Current.Value;
            while (queue.Count > 0)
            {
                IPoolObject o = queue.Dequeue() as IPoolObject;
                o.isPool = false;
                o.OnDestroy();
            }
        }
        _pool.Clear();
    }
}
