using System;
using System.Collections.Generic;
using UnityEngine;

public static class AssetPool
{
    static private Dictionary<uint, Queue<AssetObject>> _pool = new Dictionary<uint, Queue<AssetObject>>();

    /// <summary>
    /// 归还对象到对象池
    /// </summary>
    /// <param name="o">对象</param>
    static public void ReturnInstance(AssetObject asset) 
    {
        if(asset == null)
        {
            return;
        }

        if (asset.gameObject != null)
        {
            asset.gameObject.SetActive(false);
        }
       
        if(_pool.ContainsKey(asset.id)==false)
        {
            _pool.Add(asset.id, new Queue<AssetObject>());
        }
        _pool[asset.id].Enqueue(asset);
    }

    static public AssetObject GetInstance(uint id)
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
                var asset = _pool[id].Dequeue();
                if (asset.gameObject != null)
                {
                    UnityEngine.Object.Destroy(asset.gameObject);

                }
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
                var asset = it.Current.Value.Dequeue();
                if (asset.gameObject != null)
                {
                    UnityEngine.Object.Destroy(asset.gameObject);

                }
            }
        }
        _pool.Clear();
    }
}

