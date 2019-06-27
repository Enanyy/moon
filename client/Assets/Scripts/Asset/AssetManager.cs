using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public enum LoadMode
{
    Sync,
    Async,
    WWW,
}

public enum AssetMode
{
    Editor,
    AssetBundle,
}
public class LoadTask<T>
{
    public string bundleName { get; private set; }
    public string assetName { get; private set; }
    public Action<T> callback { get; private set; }

    public LoadTask(string bundleName,string assetName,Action<T> callback)
    {
        this.bundleName = bundleName;
        this.assetName = assetName;
        this.callback = callback;
    }

    public void Cancel()
    {
        callback = null;

    }
}
public class AssetManager : MonoBehaviour
{

    #region Instance
    private static AssetManager mInstance;
    public static AssetManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject(typeof(AssetManager).Name);
                mInstance = go.AddComponent<AssetManager>();
                DontDestroyOnLoad(go);
            }
            return mInstance;
        }
    }
    #endregion


    private List<LoadTask<BundleObject>> mLoadTask = new List<LoadTask<BundleObject>>();

    private AssetBundle mManifestBundle;
    private AssetBundleManifest mManifest;
    private string mAssetBundlePath;
    private Dictionary<string, BundleObject> mAssetBundleDic = new Dictionary<string, BundleObject>();

    public AssetMode assetMode { get; private set; }
    public LoadMode loadMode { get; private set; }
    public bool initialized { get; private set; }
    public void Init(LoadMode loadMode,AssetMode assetMode, string manifest)
    {
        this.loadMode = loadMode;
        this.assetMode = assetMode;

     
        if(loadMode == LoadMode.Sync)
        {
            string path = GetPath(manifest);

            var assetBundle = AssetBundle.LoadFromFile(path);

            if (assetBundle)
            {
                InitFinish(assetBundle);
            }
            else
            {
                Debug.LogError(manifest + ": Error!!");
            }
        }
        else if(loadMode == LoadMode.Async)
        {
            StartCoroutine(InitAsync(manifest));
        }
        else if(loadMode == LoadMode.WWW)
        {
            StartCoroutine(InitWWW(manifest));
        }
    }
    private IEnumerator InitAsync(string manifest)
    {
        string path = GetPath(manifest);

        var request = AssetBundle.LoadFromFileAsync(path);
        yield return request;

        if (request.isDone && request.assetBundle)
        {
            InitFinish(request.assetBundle);
        }
        else
        {
            Debug.LogError("Load assetbundle:" + manifest + " failed!!");
        }
    }
    private IEnumerator InitWWW(string manifest)
    {
        string path = GetPath(manifest);

        using (WWW www = new WWW(path))
        {
            yield return www;
            if (www.isDone && www.assetBundle)
            {
                InitFinish(www.assetBundle);
            }
            else
            {
                Debug.LogError("Load assetbundle:" + manifest + " failed!!");
            }              
        }
    }
    private void InitFinish(AssetBundle assetBundle)
    {
        mManifestBundle = assetBundle;

        mManifest = mManifestBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

        DontDestroyOnLoad(mManifest);

        initialized = true;

        for(int i = 0; i < mLoadTask.Count; ++i)
        {
            Load(mLoadTask[i].bundleName, mLoadTask[i].callback);
        }
        mLoadTask.Clear();
    }

    public LoadTask<AssetObject<T>> LoadAsset<T>(string key, System.Action<AssetObject<T>> callback)where  T:Object
    {
        var asset = AssetPath.Get(key);
        if (asset != null)
        {
            switch (asset.type)
            {
                case AssetType.Resource:
                {
                    string path = asset.path.Substring(0, asset.path.LastIndexOf('.'));
                    return LoadResource(path, callback);
                }
                case AssetType.StreamingAsset:
                {
                    return LoadAsset(string.IsNullOrEmpty(asset.group) ? 
                            asset.path : asset.group,
                            asset.path,
                            callback);
                }
                case AssetType.PersistentAsset:
                {
                    return LoadAsset(string.IsNullOrEmpty(asset.group) ? 
                        asset.path : asset.group,
                        asset.path, 
                        callback);
                }
            }
        }

        return null;
    }

    public LoadTask<AssetObject<T>> LoadResource<T>(string path,Action<AssetObject<T>> callback) where T : Object
    {
        LoadTask<AssetObject<T>> assetTask = new LoadTask<AssetObject<T>>(path, path, callback);

        LoadTask<BundleObject> bundleTask = new LoadTask<BundleObject>(path, null, delegate(BundleObject bundleObject)
        {
            AssetObject<T> assetObject = bundleObject.LoadAsset<T>(bundleObject.bundleName);
            if (assetTask.callback != null)
            {
                assetTask.callback(assetObject);
            }
           
        });

        BundleObject bundle = CreateBundle(path);
        Object obj = bundle.LoadAsset(path);
        if (obj == null)
        {
            if (bundle.onFinished.Count > 0)
            {
                bundle.onFinished.Add(bundleTask);
            }
            else
            {
                bundle.onFinished.Add(bundleTask);

                StartCoroutine(bundle.LoadResource());
            }
        }
        else
        {
            AssetObject<T> assetObject = bundle.LoadAsset<T>(path);
            if (callback != null)
            {
                callback(assetObject);
            }
        }

        return assetTask;
    }

    public LoadTask<AssetObject<T>> LoadAsset<T>(string bundleName, string assetName, System.Action<AssetObject<T>> callback = null) where T : UnityEngine.Object
    {
        LoadTask<AssetObject<T>> task = new LoadTask<AssetObject<T>>(bundleName.ToLower(), assetName.ToLower(), callback);

#if UNITY_EDITOR
        if (assetMode == AssetMode.Editor)
        {
            AssetObject<T> assetObject = null;

            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(task.assetName);

            if (asset)
            {
                if (typeof(T) == typeof(GameObject))
                {
                    if (task.callback != null)
                    {
                        var go = Instantiate(asset) as GameObject;

                        assetObject = new AssetObject<T>(task.assetName, null, asset, go as T);

                        task.callback(assetObject);
                    }
                }
                else
                {
                    if (task.callback != null)
                    {
                        assetObject = new AssetObject<T>(task.assetName, null, asset, null);
                        task.callback(assetObject);
                    }
                }
            }
            else
            {
                if (task.callback != null)
                {
                    task.callback(assetObject);
                }
            }
            return task;
        }
#endif

        Load(task.bundleName, (bundle) =>
        {
            if (bundle != null)
            {
                AssetObject<T> assetObject = bundle.LoadAsset<T>(task.assetName);

                if (task.callback != null)
                {
                    task.callback(assetObject);
                }
            }
            else
            {
                if (task.callback != null)
                {
                    task.callback(null);
                }
            }
        });

        return task;
    }


    public LoadTask<BundleObject> Load(string bundleName, Action<BundleObject> callback)
    {
        LoadTask<BundleObject> task = new LoadTask<BundleObject>(bundleName.ToLower(), null, callback);

        if (initialized == false)
        {
            mLoadTask.Add(task);

            return task;
        }

        BundleObject bundle = CreateBundle(task.bundleName);

        if (bundle.bundle == null)
        {
            if (bundle.onFinished.Count > 0)
            {
                bundle.onFinished.Add(task);
            }
            else
            {
                bundle.onFinished.Add(task);

                if (loadMode == LoadMode.Sync)
                {
                    bundle.LoadSync();
                }
                else if (loadMode == LoadMode.Async)
                {
                    StartCoroutine(bundle.LoadAsync());
                }
                else if (loadMode == LoadMode.WWW)
                {
                    StartCoroutine(bundle.LoadWWW());
                }
            }
        }
        else
        {
            if (task != null && task.callback != null)
            {
                task.callback(bundle);
            }
        }
        return task;
    }

    public BundleObject GetBundle(string bundleName)
    {
        BundleObject bundle = null;

        mAssetBundleDic.TryGetValue(bundleName, out bundle);

        return bundle;
    }

    public BundleObject CreateBundle(string bundleName)
    {
        BundleObject bundle = GetBundle(bundleName);

        if(bundle == null)
        {
            bundle = new BundleObject(bundleName, GetAllDependencies(bundleName));
            mAssetBundleDic.Add(bundleName, bundle);
        }
        return bundle;
    }

    public void RemoveBundle(BundleObject bundle)
    {
        if(bundle == null)
        {
            return;
        }
        string bundleName = bundle.bundleName;
        if(mAssetBundleDic.ContainsKey(bundleName))
        {
            mAssetBundleDic.Remove(bundleName);
        }
    }

    public string GetPath(string bundleName)
    {
        //string fullpath = GetRoot() + bundleName;
        //if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    fullpath = Uri.EscapeUriString(fullpath);
        //}
        //return fullpath;
        return AssetPath.GetPath(bundleName);
    }

    public string[] GetAllDependencies(string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName) || mManifest == null)
        {
            return null;
        }

        return mManifest.GetAllDependencies(bundleName);
    }

    public bool OtherDependence(BundleObject entity ,string bundleName)
    {    
        var it = mAssetBundleDic.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Value != entity 
                && it.Current.Value.Dependence(bundleName))
            {
                return true;
            }
        }
        it.Dispose();

        return false;
    }

    public void UnLoad(string bundleName)
    {
        BundleObject bundle = GetBundle(bundleName);
        if (bundle != null)
        {
            bundle.UnLoad();
        }
    }

    public void Destroy()
    {
        var it = mAssetBundleDic.GetEnumerator();
        while (it.MoveNext())
        {
            if(it.Current.Value.bundle!= null)
            {
                it.Current.Value.bundle.Unload(true);
            }
            it.Current.Value.references.Clear();
            it.Current.Value.dependences.Clear();
        }

        mAssetBundleDic.Clear();

        if (mManifestBundle)
        {
            mManifestBundle.Unload(true);
        }
        mManifest = null;
    }
}
