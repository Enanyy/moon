using System;
using System.Collections.Generic;
using UnityEngine;

public static class AssetPool
{
    static private Dictionary<uint, Queue<GameObject>> _pool = new Dictionary<uint, Queue<GameObject>>();

    /// <summary>
    /// 归还对象到对象池
    /// </summary>
    /// <param name="o">对象</param>
    static public void ReturnInstance(uint id, GameObject go) 
    {
        if(go == null)
        {
            return;
        }
        go.SetActive(false);
        if(_pool.ContainsKey(id)==false)
        {
            _pool.Add(id, new Queue<GameObject>());
        }
        _pool[id].Enqueue(go);
    }

    static public GameObject GetInstance(uint id)
    {
        if(_pool.ContainsKey(id))
        {
            while(_pool[id].Count > 0)
            {
                var go =  _pool[id].Dequeue();
                if(go != null)
                {
                    return go;
                }
            }
        }
        return null;
    }

    static public void Clear(uint id)
    {
        if (_pool.ContainsKey(id))
        {
            while (_pool[id].Count > 0)
            {
                GameObject go = _pool[id].Dequeue();
                UnityEngine.Object.Destroy(go);
            }
            _pool[id].Clear();
        }
    }

    static public void Clear()
    {
        var it = _pool.GetEnumerator();
        while(it.MoveNext())
        {
            while (it.Current.Value.Count > 0)
            {
                GameObject go = it.Current.Value.Dequeue();
                UnityEngine.Object.Destroy(go);
            }
        }
        _pool.Clear();
    }
}

