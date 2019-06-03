using UnityEngine;
using System;
using System.Collections.Generic;

public class AssetManager 
{
    private static AssetManager _instance;
    public static AssetManager Instance
    {
        get
        {
            if(_instance==null)
            {
                _instance = new AssetManager();
            }
            return _instance;
        }
    }

    private Dictionary<uint, Queue<AssetObject>> mCachePool = new Dictionary<uint, Queue<AssetObject>>();

    private Dictionary<uint,List<AssetObject>> mAssetDic = new Dictionary<uint, List<AssetObject>>(); 
    /// <summary>
    /// 归还对象到对象池
    /// </summary>
    /// <param name="o">对象</param>
     public void ReturnInstance(AssetObject asset)
    {
        if (asset == null)
        {
            return;
        }

        if (asset.gameObject != null)
        {
            asset.gameObject.SetActive(false);
        }

        if (mCachePool.ContainsKey(asset.id) == false)
        {
            mCachePool.Add(asset.id, new Queue<AssetObject>());
        }
        mCachePool[asset.id].Enqueue(asset);
    }

    private AssetObject GetInstance(uint id)
    {
        if (mCachePool.ContainsKey(id))
        {
            while (mCachePool[id].Count > 0)
            {
                var go = mCachePool[id].Dequeue();
                if (go != null)
                {
                    return go;
                }
            }
        }
        return null;
    }

    public void Clear(uint id)
    {
        if (mAssetDic.ContainsKey(id))
        {
            var list = mAssetDic[id];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].gameObject)
                {
                    UnityEngine.Object.DestroyImmediate(list[i].gameObject);
                }
            }
            list.Clear();

            mAssetDic.Remove(id);
        }

        if (mCachePool.ContainsKey(id))
        {      
            mCachePool[id].Clear();
        }
    }

    public void Clear()
    {
        var it = mAssetDic.GetEnumerator();
        while (it.MoveNext())
        {
            var list = it.Current.Value;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].gameObject)
                {
                    UnityEngine.Object.DestroyImmediate(list[i].gameObject);
                }
            }

            list.Clear();
        }

        mAssetDic.Clear();
        mCachePool.Clear();
    }

    public void Instantiate(uint id,Action<AssetObject> callback)
    {
        var assetObject = GetInstance(id);
        if (assetObject != null)
        {
            if (assetObject.gameObject == null)
            {
                assetObject.gameObject = UnityEngine.Object.Instantiate(assetObject.obj) as GameObject;
            }

            if (callback != null)
            {
                callback(assetObject);
            }
        }
        else
        {
            if (mAssetDic.ContainsKey(id) && mAssetDic[id].Count > 0)
            {
                assetObject = new AssetObject
                {
                    id = id,
                    obj = mAssetDic[id][0].obj,
                    gameObject = UnityEngine.Object.Instantiate(mAssetDic[id][0].obj) as GameObject

                };
                mAssetDic[id].Add(assetObject);
                if (callback != null)
                {
                    callback(assetObject);
                }
            }
            else
            {
                Load(id, (asset) =>
                {
                    if (asset != null)
                    {
                        if (asset.gameObject == null)
                        {
                            asset.gameObject = UnityEngine.Object.Instantiate(asset.obj) as GameObject;
                        }
                    }

                    if (callback != null)
                    {
                        callback(asset);
                    }
                });
            }
        }
    }

    public void Load(uint id, Action<AssetObject> callback)
    {
        var asset = GetInstance(id);
        if (asset == null)
        {
            string assetPath = AssetPath.Get(id);

            asset = new AssetObject
            {
                id = id,
                obj = Resources.Load<UnityEngine.Object>(assetPath),
            };

            if (mAssetDic.ContainsKey(id) == false)
            {
                mAssetDic.Add(id,new List<AssetObject>());
            }
            mAssetDic[id].Add(asset);

            if (callback != null)
            {
                callback(asset);
            }
        }
        else
        {
            if (callback != null)
            {
                callback(asset);
            }
        }
    }

    public void Release(AssetObject asset)
    {
        if (asset == null)
        {
            return;
        }

        if (asset.gameObject != null)
        {
            UnityEngine.Object.DestroyImmediate(asset.gameObject);
        }

        asset.gameObject = null;

        if (mAssetDic.ContainsKey(asset.id))
        {
            var list = mAssetDic[asset.id];
            for (int i = list.Count -1; i >= 0; i--)
            {
                if (list[i] == asset)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}