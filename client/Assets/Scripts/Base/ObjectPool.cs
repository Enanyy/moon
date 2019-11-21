using System;
using System.Collections.Generic;

public interface IPoolObject
{
    /// <summary>
    /// 构造（创建）
    /// </summary>
    void OnConstruct();

    /// <summary>
    /// 析构(返回对象池)
    /// </summary>
    void OnDestruct(); 
}
/// <summary>
/// 对象池,用于保存经常用并可重复用的对象
/// </summary>
public class ObjectPool
{
    /// <summary>
    /// 每个类型的对象池最大容量
    /// </summary>
    public const int maxNum = 256; 

    static private Dictionary<Type, List<IPoolObject>> mPool = new Dictionary<Type, List<IPoolObject>>();
    /// <summary>
    /// 归还对象到对象池
    /// </summary>
    /// <param name="o">对象</param>
	static public void ReturnInstance<T>(T obj, Type type = null) where T : IPoolObject
    {
        if (obj == null)
            return;
        if (type == null)
            type = typeof(T);
        List<IPoolObject> list;
        if (!mPool.TryGetValue(type, out list))
            list = AddType(type);
        if (list.Contains(obj))
        {
            UnityEngine.Debug.LogError(string.Format("ReturnInstance Exists:type={0}", type));
            return;
        }
        if (list.Count > maxNum)
        {
            UnityEngine.Debug.LogError(string.Format("ReturnInstance maxNum:type={0}", type));
            return;
        }
        obj.OnDestruct();
        list.Add(obj);
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
        T obj;
        if (list.Count > 0)
        {
            obj = (T)list[0];
            list.RemoveAt(0);
        }
        else
        {
            obj = (T)Activator.CreateInstance(type);// type.Assembly.CreateInstance(type.FullName);
        }
        obj.OnConstruct();
        return obj;
    }
    /// <summary>
    /// 是否在对象池内
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static public bool IsInPool(IPoolObject obj)
    {
        Type type = obj.GetType();
        List<IPoolObject> list;
        if (mPool.TryGetValue(type, out list))
        {
            return list.Contains(obj);
        }
        return false;
    }

    /// <summary>
    /// 清理指定类型type的对象池
    /// </summary>
    /// <param name="service">类型</param>
    static public void Clear(Type type,bool includeSubClass = false)
    {
        List<IPoolObject> list;
        if (mPool.TryGetValue(type, out list))
        {
            list.Clear();
            mPool.Remove(type);
        }

        if (includeSubClass)
        {
            var it = mPool.GetEnumerator();
            List<Type> removes = new List<Type>();
            while (it.MoveNext())
            {
                if(it.Current.Key.IsSubclassOf(type))
                {
                    removes.Add(it.Current.Key);
                    it.Current.Value.Clear();
                }
            }
            for (int i = 0; i < removes.Count; ++i)
            {
                mPool.Remove(removes[i]);
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
    }
}
