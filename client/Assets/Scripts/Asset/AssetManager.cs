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

    public void Instantiate(uint id,Action<uint,GameObject> callback)
    {
        GameObject gameObject = AssetPool.GetInstance(id);
        if (gameObject == null)
        {
            string assetPath = AssetPath.Get(id);
            UnityEngine.Object obj = Resources.Load<UnityEngine.Object>(assetPath);
            gameObject = UnityEngine.Object.Instantiate(obj) as GameObject;

            if(callback!= null)
            {
                callback(id, gameObject);
            }
        }
        else
        {
            if (callback != null)
            {
                callback(id, gameObject);
            }
        }
    }

    public void Load(uint id, Action<uint, UnityEngine.Object> callback)
    {
        string assetPath = AssetPath.Get(id);
        UnityEngine.Object obj = Resources.Load<UnityEngine.Object>(assetPath);
        if(callback!= null)
        {
            callback(id, obj);
        }
    }
}