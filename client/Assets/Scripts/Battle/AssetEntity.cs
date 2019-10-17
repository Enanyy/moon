//////////////////////////////////////////////////////////////////////////
// 
// 文件：Scripts\Battle\AssetEntity.cs
// 作者：Lee
// 时间：2019/01/21
// 描述：战斗表现层基类，包括战斗特效
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
public class AssetEntity :IPoolObject
{
    public bool isPool { get ; set ; }

    public GameObject gameObject { get; private set; }

    public Asset<GameObject> asset { get; private set; }
    public AssetEntity()
    {
        
    }

    protected void LoadAsset(string key)
    {      
        AssetManager.Instance.LoadAsset<GameObject>(key,OnAssetLoad);
    }

    protected virtual void OnAssetLoad(Asset<GameObject> asset)
    {   
        if(asset!=null)
        {
            this.asset = asset;
            this.gameObject = asset.assetObject;
        }

        if (gameObject != null)
        {        
            gameObject.SetActive(true);
            
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
            SetLayer();

        }

    }

    public void SetLayer()
    {
        if (gameObject != null)
        {
            int layer = LayerMask.NameToLayer("Default");
            gameObject.SetLayer(layer, true);
        }
    }


    public virtual void OnCreate()
    {
       
    }

    public virtual void OnDestroy()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
            
        }
        if (asset != null)
        {
            asset.ReturnAsset();
        }
        asset = null;
    }

    public virtual void OnReturn()
    {
        if (gameObject != null)
        { 
            gameObject.SetActive(false);
        }
        if (asset != null)
        {
            asset.ReturnAsset();
        }
        asset = null;
    }
    public virtual void Recycle()
    {
        ObjectPool.ReturnInstance(this, GetType());
    }
}

