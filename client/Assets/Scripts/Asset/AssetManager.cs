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

    private Dictionary<string, Queue<AssetObject>> mCachePool = new Dictionary<string, Queue<AssetObject>>();

    private Dictionary<string, List<AssetObject>> mAssetDic = new Dictionary<string, List<AssetObject>>(); 
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

        if (mCachePool.ContainsKey(asset.name) == false)
        {
            mCachePool.Add(asset.name, new Queue<AssetObject>());
        }
        mCachePool[asset.name].Enqueue(asset);
    }

    private AssetObject GetInstance(string name)
    {
        if (mCachePool.ContainsKey(name))
        {
            while (mCachePool[name].Count > 0)
            {
                var go = mCachePool[name].Dequeue();
                if (go != null)
                {
                    return go;
                }
            }
        }
        return null;
    }

    public void Clear(string name)
    {
        if (mAssetDic.ContainsKey(name))
        {
            var list = mAssetDic[name];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].gameObject)
                {
                    UnityEngine.Object.DestroyImmediate(list[i].gameObject);
                }
            }
            list.Clear();

            mAssetDic.Remove(name);
        }

        if (mCachePool.ContainsKey(name))
        {      
            mCachePool[name].Clear();
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

    public void Instantiate(string name,Action<AssetObject> callback)
    {
        var assetObject = GetInstance(name);
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
            if (mAssetDic.ContainsKey(name) && mAssetDic[name].Count > 0)
            {
                assetObject = new AssetObject
                {
                    name = name,
                    obj = mAssetDic[name][0].obj,
                    gameObject = UnityEngine.Object.Instantiate(mAssetDic[name][0].obj) as GameObject

                };
                mAssetDic[name].Add(assetObject);
                if (callback != null)
                {
                    callback(assetObject);
                }
            }
            else
            {
                Load(name, (asset) =>
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

    public void Load(string name, Action<AssetObject> callback)
    {
        var asset = GetInstance(name);
        if (asset == null)
        {
            string assetPath = AssetPath.Get(name);

            asset = new AssetObject
            {
                name = name,
                obj = Resources.Load<UnityEngine.Object>(assetPath.Contains(".")? assetPath.Substring(0,assetPath.LastIndexOf('.')):assetPath),
            };

            if (mAssetDic.ContainsKey(name) == false)
            {
                mAssetDic.Add(name,new List<AssetObject>());
            }
            mAssetDic[name].Add(asset);

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

        if (mAssetDic.ContainsKey(asset.name))
        {
            var list = mAssetDic[asset.name];
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