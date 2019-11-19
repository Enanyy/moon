using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// 通用预设管理
/// </summary>

public enum AssetPrefabID
{
    Item = 0,
}
[Serializable]
public class AssetPrefab
{
    public AssetPrefabID id;
    public Object obj;

    [HideInInspector]
    public List<Object> caches = new List<Object>();
}

public class AssetCache : MonoBehaviour
{
    private static AssetCache mInstance;

    private static Asset<GameObject> mAsset;

    private static bool mInitializing = false;
    public static void Initialize()
    {
        if(mInstance!=null)
        {
            return;
        }
        if(mInitializing)
        {
            return;
        }
        mInitializing = true;

        AssetManager.Instance.LoadAsset<GameObject>("assetcache.prefab", (asset) => { 
        
            if(asset!= null)
            {
                mAsset = asset;
                mInstance = mAsset.assetObject.GetComponent<AssetCache>();
                DontDestroyOnLoad(mAsset.assetObject);
            }
            mInitializing = false;
        
        });
    }

    public static void Destroy()
    {
        if(mInstance!= null)
        {
            Destroy(mInstance.gameObject);
        }
        if(mAsset!= null)
        {
            mAsset.Destroy();
        }
        mInstance = null;
        mAsset = null;
        mInitializing = false;
    }

    [SerializeField]
    private List<AssetPrefab> Prefabs = new List<AssetPrefab>();

    public static AssetPrefab Get(AssetPrefabID id)
    {
        if(mInstance == null)
        {
            return null;
        }
        for(int i = 0; i < mInstance.Prefabs.Count; ++i )
        {
            if(mInstance.Prefabs[i].id == id)
            {
                return mInstance.Prefabs[i];
            }
        }
        return null;
    }

    public static GameObject Create(AssetPrefabID id, Transform parent)
    {
        AssetPrefab prefab = Get(id);
        if(prefab!= null)
        {

            GameObject go = null;
            if (prefab.caches.Count > 0)
            {
                go = prefab.caches[0] as GameObject;
                prefab.caches.RemoveAt(0);
            }
            else
            { 
                go = Instantiate(prefab.obj) as GameObject;
            }
            go.SetActive(true);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            return go;
        }
        return null;
    }

    public void Return(AssetPrefabID id, GameObject go)
    {
        if(go == null)
        {
            return;
        }
        AssetPrefab prefab = Get(id);
        if(prefab!= null)
        {
            go.transform.SetParent(mInstance.transform);
            go.SetActive(false);
            prefab.caches.Add(go);
        }
        else
        {
            Destroy(go);
        }
    }

}
