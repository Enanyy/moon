﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.IO;

public class LoadTask 
{
    public string key { get; private set; }

    public string bundleName
    {
        get
        {
            if (path != null)
            {
                return string.IsNullOrEmpty(path.group) ? path.path : path.group;
            }
            return key;
        }
    }

    public string assetName
    {
        get
        {
            if (path != null)
            {
                return path.path;
            }
            return key;
        }
    }
    private AssetPath mPath;
    public AssetPath path
    {
        get
        {
            if (mPath == null)
            {
                mPath = AssetPath.Get(key);
                //Debug.Log(mPath.path);
            }
            return mPath;
        }
    }
    public LoadTask(string key)
    {
        this.key = key;

    }
    public bool isCancel { get; set; }
    public void Cancel()
    {
        isCancel = true;
    }
}

public class SceneLoadTask : LoadTask
{
    public LoadSceneMode mode { get; set; }
    public Action<Scene, LoadSceneMode> callback { get; private set; }

    public string sceneName { get { return Path.GetFileNameWithoutExtension(assetName); } }
    public SceneLoadTask(string key, LoadSceneMode mode, Action<Scene, LoadSceneMode> callback) : base(key)
    {
        this.mode = mode;
        this.callback = callback;
    }
    public void OnCompleted(Scene scene, LoadSceneMode mode)
    {
        if (callback != null)
        {
            callback(scene, mode);
        }
    }
}

public class AssetLoadTask<T> : LoadTask
{
    public Action<T> callback { get; private set; }
    public AssetLoadTask(string key, Action<T> callback) : base(key)
    {
        this.callback = callback;
    }
    public void OnCompleted(T t)
    {
        if (callback != null)
        {
            callback(t);
        }
    }
}