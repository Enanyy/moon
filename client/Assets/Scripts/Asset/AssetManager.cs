using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.IO;

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

    public LoadTask(string bundleName, string assetName, Action<T> callback)
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

    private AssetBundle mManifestBundle;
    private AssetBundleManifest mManifest;
    private string mAssetBundlePath;
    private Dictionary<string, Bundle> mAssetBundleDic = new Dictionary<string, Bundle>();

    public AssetMode assetMode { get; private set; }
    public LoadMode loadMode { get; private set; }
    public bool initialized { get; private set; }

    private bool mInitializing = false;

    private List<Action> mLoadTask = new List<Action>();
    
    public void Init()
    {
        if (initialized || mInitializing)
        {
            return;
        }

        mInitializing = true;
#if UNITY_EDITOR
        string path = string.Format("{0}{1}", AssetPath.persistentDataPath, AssetPath.ASSETS_FILE);

        AssetPath.mode = (AssetMode)PlayerPrefs.GetInt("assetMode");
        if (AssetPath.mode == AssetMode.Editor)
        {
            path = string.Format("{0}/{1}", Application.dataPath,AssetPath.ASSETS_FILE);
            string xml = File.ReadAllText(path);
            AssetPath.FromXml(xml);
            InitFinish(null);
        }
        else
        {
            StartCoroutine(InitAssets(path));
        }
#else
		string path = string.Format("{0}{1}", AssetPath.persistentDataPath, AssetPath.ASSETS_FILE);
        if (File.Exists(path) == false)
        {
            path = string.Format("{0}{1}", AssetPath.streamingAssetsPath, AssetPath.ASSETS_FILE);
        }
        AssetPath.mode = AssetMode.AssetBundle;
        StartCoroutine(InitAssets(path));
#endif
    }

    private IEnumerator InitAssets(string path)
    {
        using (WWW www = new WWW(path))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.text) == false)
            {
                AssetPath.FromXml(www.text);
                Init(LoadMode.Async, AssetPath.mode, AssetPath.manifest);
            }
            else
            {
                Debug.LogError(www.error+":"+www.url);
            }          
        }
    }

    public void Init(LoadMode loadMode, AssetMode assetMode, string manifest)
    {
        this.loadMode = loadMode;
        this.assetMode = assetMode;

        switch (loadMode)
        {
            case LoadMode.Sync: InitSync(manifest);break;   
            case LoadMode.Async: StartCoroutine(InitAsync(manifest));break;        
            case LoadMode.WWW: StartCoroutine(InitWWW(manifest));break;
        }
    }

    private void InitSync(string manifest)
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
        if (assetBundle != null)
        {
            mManifestBundle = assetBundle;

            mManifest = mManifestBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

            DontDestroyOnLoad(mManifest);
        }

        initialized = true;
        mInitializing = false;

        for (int i = 0; i < mLoadTask.Count; i++)
        {
            mLoadTask[i]();
        }
        mLoadTask.Clear();
    }

    public LoadTask<Asset<T>> LoadAsset<T>(string key, Action<Asset<T>> callback)where  T:Object
    {
        if (initialized == false)
        {
            Init();
            mLoadTask.Add(new Action(() => LoadAsset(key,callback)));
            return null;
        }
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
                case AssetType.PersistentAsset:
                {
                    return LoadAsset(string.IsNullOrEmpty(asset.group) ? 
                            asset.path : asset.group,
                            asset.path,
                            callback);
                }
            }
        }
        else
        {
            Debug.LogError("Can not find AssetPath by:" + key);
        }

        return null;
    }
   


    public LoadTask<Asset<T>> LoadResource<T>(string path,Action<Asset<T>> callback) where T : Object
    {
        LoadTask<Asset<T>> task = new LoadTask<Asset<T>>(path, path, callback);

        LoadTask<Bundle> bundleTask = new LoadTask<Bundle>(task.bundleName, null, delegate (Bundle bundleObject)
        {
            Asset<T> assetObject = bundleObject.LoadAsset<T>(bundleObject.bundleName);
            if (task.callback != null)
            {
                task.callback(assetObject);
            }
        });

        Bundle bundle = CreateBundle(task.bundleName);
        Object obj = bundle.LoadAsset(task.assetName);
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
            Asset<T> assetObject = bundle.LoadAsset<T>(task.assetName);
            if (task.callback != null)
            {
                task.callback(assetObject);
            }
        }

        return task;
    }

   

    public LoadTask<Asset<T>> LoadAsset<T>(string bundleName, string assetName, System.Action<Asset<T>> callback = null) where T : UnityEngine.Object
    {
        LoadTask<Asset<T>> task = new LoadTask<Asset<T>>(bundleName.ToLower(), assetName.ToLower(), callback);

#if UNITY_EDITOR
        if (assetMode == AssetMode.Editor)
        {
            Asset<T> assetObject = null;

            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(task.assetName);

            if (asset)
            {
                if (typeof(T) == typeof(GameObject))
                {
                    if (task.callback != null)
                    {
                        var go = Instantiate(asset) as GameObject;

                        assetObject = new Asset<T>(task.assetName, null, asset, go as T);

                        task.callback(assetObject);
                    }
                }
                else
                {
                    if (task.callback != null)
                    {
                        assetObject = new Asset<T>(task.assetName, null, asset, null);
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
                if (task.callback != null)
                {
                    Asset<T> assetObject = bundle.LoadAsset<T>(task.assetName);
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

    
    public LoadTask<Bundle> Load(string bundleName, Action<Bundle> callback)
    {
        if (initialized == false)
        {
            Init();
            mLoadTask.Add(new Action(()=>Load(bundleName,callback)));
            return null;
        }

        LoadTask<Bundle> task = new LoadTask<Bundle>(bundleName.ToLower(), null, callback);

        Bundle bundle = CreateBundle(task.bundleName);

        if (bundle.bundle == null)
        {
            if (bundle.onFinished.Count > 0)
            {
                bundle.onFinished.Add(task);
            }
            else
            {
                bundle.onFinished.Add(task);

                switch (loadMode)
                {
                    case LoadMode.Sync: bundle.LoadSync();break;
                    case LoadMode.Async: StartCoroutine(bundle.LoadAsync());break;
                    case LoadMode.WWW: StartCoroutine(bundle.LoadWWW());break;
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

    

    public Bundle GetBundle(string bundleName)
    {
        Bundle bundle = null;

        mAssetBundleDic.TryGetValue(bundleName, out bundle);

        return bundle;
    }

    public Bundle CreateBundle(string bundleName)
    {
        Bundle bundle = GetBundle(bundleName);

        if(bundle == null)
        {
            bundle = new Bundle(bundleName, GetAllDependencies(bundleName));
            mAssetBundleDic.Add(bundleName, bundle);
        }
        return bundle;
    }

    public void RemoveBundle(Bundle bundle)
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
        string path = AssetPath.GetPath(bundleName);
        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            path = Uri.EscapeUriString(path);
        }
       
        return path;
    }

    public string[] GetAllDependencies(string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName) || mManifest == null)
        {
            return null;
        }

        return mManifest.GetAllDependencies(bundleName);
    }

    public bool OtherDependence(Bundle entity ,string bundleName)
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
        return false;
    }

    public void UnLoad(string bundleName)
    {
        Bundle bundle = GetBundle(bundleName);
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
