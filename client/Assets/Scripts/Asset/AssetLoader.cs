﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AssetLoader : MonoBehaviour
{
    #region 静态公有方法
    /// <summary>
    /// 60s自动检测已经销毁的资源，并释放对应的Bundle
    /// </summary>
    public static float unloadInterval = 60;

    /// <summary>
    /// key可以是文件名(带后缀)或者文件路径(Assets/...)。
    /// 注意，当非实例化资源（非GameObject，如Material，Texture，TextAsset等）使用完毕后,需要主动调用Destroy去释放资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static AssetLoadTask<T> LoadAsset<T>(string key, Action<Asset<T>> callback) where T : UnityEngine.Object
    {
        AssetLoadTask<T> task = new AssetLoadTask<T>(key.ToLower(), callback);

        LoadAsset(task);

        return task;
    }
    /// <summary>
    /// 自定义LoadTask。当非实例化资源（非GameObject，如Material，Texture，TextAsset等）使用完毕后,需要主动调用Destroy去释放资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    public static void LoadAsset<T>(IAssetLoadTask<T> task) where T : UnityEngine.Object
    {
        if (task == null)
        {
            return;
        }

        Instance.StartCoroutine(Instance.LoadAssetAsync(task));
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="key"></param>
    /// <param name="mode"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static SceneLoadTask LoadScene(string key, LoadSceneMode mode, Action<Scene, LoadSceneMode> callback)
    {
        SceneLoadTask task = new SceneLoadTask(key.ToLower(), mode, callback);
        LoadScene(task);
        return task;
    }
    /// <summary>
    /// 自定义LoadTask
    /// </summary>
    /// <param name="task"></param>
    public static void LoadScene(ISceneLoadTask task)
    {
        if (task == null)
        {
            return;
        }
        Instance.StartCoroutine(Instance.LoadSceneAsync(task));
    }
  

    /// <summary>
    /// 卸载一个Additive场景
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="callback"></param>
    public static void UnLoadScene(Scene scene, Action callback)
    {
        Instance.StartCoroutine(Instance.UnloadSceneAsync(scene, callback));
    }


    public static Bundle GetOrCreateBundle(string bundleName)
    {
        Bundle bundle;
        Instance.mAssetBundleDic.TryGetValue(bundleName, out bundle);

        if(bundle == null)
        {
            bundle = new Bundle(bundleName);
            Instance.mAssetBundleDic.Add(bundleName, bundle);
        }
        return bundle;
    }

    public static void RemoveBundle(Bundle bundle)
    {
        if (bundle == null || string.IsNullOrEmpty(bundle.bundleName))
        {
            return;
        }

        Instance.mAssetBundleDic.Remove(bundle.bundleName);
    }

   
    /// <summary>
    /// 获取直接依赖
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static string[] GetDirectDependencies(string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName) || Instance.mAssetManifest == null || Instance.mAssetManifest.assetObject == null)
        {
            return null;
        }

        return Instance.mAssetManifest.assetObject.GetDirectDependencies(bundleName);
    }

    public static void UnLoad(string bundleName)
    {
        Bundle bundle;
        if(Instance.mAssetBundleDic.TryGetValue(bundleName, out bundle))
        {
            bundle.UnLoad();
        }
    }

    public static void Destroy()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            mInstance = null;
        }
    }
    #endregion

    #region 私有方法
    #region Instance
    private static AssetLoader mInstance;
    private static AssetLoader Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject(typeof(AssetLoader).Name);
                mInstance = go.AddComponent<AssetLoader>();
                DontDestroyOnLoad(go);
            }
            return mInstance;
        }
    }
    #endregion
    private Asset<AssetBundleManifest> mAssetManifest;

    private Dictionary<string, Bundle> mAssetBundleDic = new Dictionary<string, Bundle>();

    private LoadStatus mStatus = LoadStatus.None;

    private List<string> mBundleNameList = new List<string>();
    private float mLastUnloadTime;


    private List<ISceneLoadTask> mSceneLoadTasks = new List<ISceneLoadTask>();


    private IEnumerator BeginInitialize()
    {
        if (mStatus == LoadStatus.Done)
        {
            yield break;
        }
        else if (mStatus == LoadStatus.Loading)
        {
            yield return new WaitUntil(() => mStatus == LoadStatus.Done);
        }
        else
        {
            //初始化资源列表
            yield return AssetPath.Initialize();

            if (AssetPath.mode == AssetMode.AssetBundle)
            {
                Bundle bundle = GetOrCreateBundle(AssetPath.list.manifest);

                AssetLoadTask<AssetBundleManifest> task = new AssetLoadTask<AssetBundleManifest>(AssetPath.list.manifest, FinishInitialize);

                task.assetName = "AssetBundleManifest";

                yield return bundle.LoadAsset(task);

            }
            else
            {
                FinishInitialize(null);
            }
        }
    }
    private void FinishInitialize(Asset<AssetBundleManifest> asset)
    {
        mAssetManifest = asset;

        if (mAssetManifest != null && mAssetManifest.assetObject != null)
        {
            DontDestroyOnLoad(mAssetManifest.assetObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;

        mStatus = LoadStatus.Done;

        Debug.Log("Initialize AssetManager Finish!");
    }

    private IEnumerator LoadAssetAsync<T>(IAssetLoadTask<T> task) where T : UnityEngine.Object
    {
        if (mStatus != LoadStatus.Done)
        {
            yield return BeginInitialize();
        }


        Bundle bundle = GetOrCreateBundle(task.bundleName);

        yield return bundle.LoadAsset(task);
    }

    private IEnumerator LoadSceneAsync(ISceneLoadTask task)
    {
        if (mStatus != LoadStatus.Done)
        {
            yield return BeginInitialize();
        }

        mSceneLoadTasks.Add(task);

        if (AssetPath.mode == AssetMode.AssetBundle)
        {
            Bundle bundle = GetOrCreateBundle(task.assetName);

            yield return bundle.LoadBundleAsync();

            yield return SceneManager.LoadSceneAsync(task.sceneName, task.mode);

        }
        else
        {
            yield return SceneManager.LoadSceneAsync(task.sceneName);
        }
    }

    private IEnumerator UnloadSceneAsync(Scene scene, Action callback)
    {
        var request = SceneManager.UnloadSceneAsync(scene);
        yield return request;
        if (callback != null)
        {
            callback();
        }
    }

    private void Update()
    {
        if (Time.time - mLastUnloadTime > unloadInterval)
        {
            mLastUnloadTime = Time.time;

            mBundleNameList.AddRange(mAssetBundleDic.Keys);

            for (int i = 0; i < mBundleNameList.Count; i++)
            {
                Bundle bundle;
                if (mAssetBundleDic.TryGetValue(mBundleNameList[i], out bundle))
                {
                    bundle.RemoveReference();
                }
            }
            mBundleNameList.Clear();
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < mSceneLoadTasks.Count;)
        {
            var task = mSceneLoadTasks[i];
            if (task.isCancel)
            {
                if (task.sceneName == scene.name && mode == LoadSceneMode.Additive)
                {
                    UnLoadScene(scene, null);
                }
                mSceneLoadTasks.RemoveAt(i);
                continue;
            }
            if (task.sceneName == scene.name)
            {
                task.OnCompleted(scene, mode);
                mSceneLoadTasks.RemoveAt(i);
                continue;
            }
            i++;
        }
    }
    private void OnDestroy()
    {
        var it = mAssetBundleDic.GetEnumerator();
        while (it.MoveNext())
        {
            var bunlde = it.Current.Value;
            if (bunlde.bundle != null)
            {
                bunlde.bundle.Unload(true);
            }
            bunlde.references.Clear();
            bunlde.dependences.Clear();
            bunlde.dependencesby.Clear();
        }

        mAssetBundleDic.Clear();
        mAssetManifest = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        mStatus = LoadStatus.None;
    }
    #endregion
}