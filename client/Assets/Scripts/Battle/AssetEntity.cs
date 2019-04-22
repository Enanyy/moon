﻿//////////////////////////////////////////////////////////////////////////
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
public class AssetEntity : IPoolObject
{
    public bool isPool { get ; set ; }

    public GameObject gameObject { get; private set; }
    
    public uint assetID { get; private set; }
    public AssetEntity()
    {
        
    }

    protected void LoadAsset(uint assetID)
    {
        this.assetID = assetID;

        gameObject = AssetPool.GetInstance(assetID);
        if (gameObject == null)
        {
            string assetPath = AssetPath.Get(assetID);
            UnityEngine.Object obj = Resources.Load<UnityEngine.Object>(assetPath);
            gameObject = UnityEngine.Object.Instantiate(obj) as GameObject;
        }
        OnAssetLoad();
    }

    protected virtual bool OnAssetLoad()
    {   
        if (gameObject != null)
        {        
            gameObject.SetActive(true);
            
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
            SetLayer();

            return true;
        }

        return false;
    }

    public void SetLayer()
    {
        if (gameObject != null)
        {
            int layer = LayerMask.NameToLayer("Default");
            gameObject.layer = layer;
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transforms.Length; ++i)
            {
                transforms[i].gameObject.layer = layer;
            }
        }
    }


    public virtual void OnCreate()
    {
       
    }

    public virtual void OnDestroy()
    {
        if (gameObject != null)
        {
            AssetPool.ReturnInstance(assetID, gameObject);
        }
        assetID = 0;
    }

    public virtual void OnReturn()
    {
        if (gameObject != null)
        {
            AssetPool.ReturnInstance(assetID, gameObject);
        }
        assetID = 0;
    }
    public virtual void Recycle()
    {
        ObjectPool.ReturnInstance(this, GetType());
    }
}

