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
    public GameObject gameObject { get; private set; }

    public Asset<GameObject> asset { get; private set; }

    private Action<GameObject> mAssetLoadAction;
    public AssetEntity()
    {

    }

    public void LoadAsset(string key, Action<GameObject> assetLoadAction = null)
    {
        mAssetLoadAction = assetLoadAction;
        AssetLoader.LoadAsset<GameObject>(key, OnAssetLoad);
    }

    public virtual void OnAssetLoad(Asset<GameObject> asset)
    {
        if (asset != null)
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

        if (mAssetLoadAction != null)
        {
            mAssetLoadAction(gameObject);
        }
    }

    public void SetLayer()
    {
        if (gameObject != null)
        {
            int layer = LayerMask.NameToLayer("Default");
            gameObject.SetLayerEx(layer, true);
        }
    }


    public virtual void OnConstruct()
    {

    }

    public virtual void OnDestruct()
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

