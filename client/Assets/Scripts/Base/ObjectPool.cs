using System;
using System.Collections.Generic;

public interface IPoolObject
{
    /// <summary>
    /// 是否是已经回收的
    /// </summary>
    bool isPool { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    void OnConstruct();

    /// <summary>
    /// 析构
    /// </summary>
    void OnDestruct(); 
}
/// <summary>
/// 对象池,用于保存经常用并可重复用的对象
/// </summary>
public class ObjectPool
{
    public static int maxNum = 8196;

    static private Dictionary<Type, List<IPoolObject>> mPool = new Dictionary<Type, List<IPoolObject>>();
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
        List<IPoolObject> queue;
        if (!mPool.TryGetValue(type, out queue))
            queue = AddType(type);
        if (queue.Contains(o))
        {
            UnityEngine.Debug.LogError(string.Format("ReturnInstance Exists:type={0}", type));
            return;
        }
        if (queue.Count > maxNum)
        {
            UnityEngine.Debug.LogError(string.Format("ReturnInstance maxNum:type={0}", type));
            return;
        }
        o.OnDestruct();
        o.isPool = true;
        queue.Add(o);
    }
    static private List<IPoolObject> AddType(Type type)
    {
        List<IPoolObject> list = new List<IPoolObject>();
        mPool.Add(type, list);
        return list;
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
        List<IPoolObject> list;
        if (!mPool.TryGetValue(type, out list))
        {
            list = AddType(type);
        }
        int len = list.Count;
        T obj;
        if (len > 0)
        {
            obj = (T)list[0];
            list.RemoveAt(0);
        }
        else
        {
            obj = (T)Activator.CreateInstance(type);// type.Assembly.CreateInstance(type.FullName);
        }
        obj.OnConstruct();
        obj.isPool = false;
        return obj;
    }

    /// <summary>
    /// 清理指定类型type的对象池
    /// </summary>
    /// <param name="service">类型</param>
    static public void Clear(Type type,bool includeSubClass = false)
    {
        List<IPoolObject> l = null;
        if (mPool.TryGetValue(type, out l))
        {
            var it = l.GetEnumerator();
            while(it.MoveNext())
            {
                IPoolObject o = it.Current;
                if(o!=null)
                {
                    o.OnDestruct();
                    o.isPool = false;
                }
            }

            l.Clear();
            mPool.Remove(type);
        }

        if (includeSubClass)
        {
            var it = mPool.GetEnumerator();
            List<Type> list = new List<Type>();
            while (it.MoveNext())
            {
                if(it.Current.Key.IsSubclassOf(type))
                {
                    list.Add(it.Current.Key);

                    for(int i = 0; i < it.Current.Value.Count; ++i)
                    {
                        it.Current.Value[i].OnDestruct();
                    }
                    it.Current.Value.Clear();
                }
            }
            for (int i = 0; i < list.Count; ++i)
            {
                mPool.Remove(list[i]);
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
        mPool.Clear();

        var it = mPool.GetEnumerator();
        while (it.MoveNext())
        {
            for (int i = 0; i < it.Current.Value.Count; ++i)
            {
                it.Current.Value[i].OnDestruct();
                it.Current.Value[i].isPool = false;
            }
            it.Current.Value.Clear();
        }
        mPool.Clear();
    }
}
