using UnityEngine;
using System;

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

    public void Instantiate(uint id,Action<AssetObject> callback)
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

    public void Load(uint id, Action<AssetObject> callback)
    {
        var asset = AssetPool.GetInstance(id);
        if (asset == null)
        {
            string assetPath = AssetPath.Get(id);

            asset = new AssetObject
            {
                id = id,
                obj = Resources.Load<UnityEngine.Object>(assetPath),
            };

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
}