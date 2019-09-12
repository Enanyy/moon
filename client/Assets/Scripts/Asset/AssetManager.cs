using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;
public enum AssetMode
{
    Editor,
    AssetBundle,
}

public class LoadTask<T> 
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
                if (path.type == AssetType.Resource)
                {
                    if (path.path.Contains("."))
                    {
                        return path.path.Substring(0, path.path.LastIndexOf('.'));
                    }
                }
                return path.path;
            }
            return key;
        }
    }

    public Action<T> callback { get; private set; }

    private AssetPath mPath;
    public AssetPath path {
        get
        {
            if (mPath == null)
            {
                mPath= AssetPath.Get(key);
            }
            return mPath;
        }
    }

    public AssetType type
    {
        get { return path != null ? path.type : AssetType.Resource; }
    }

    public LoadTask(string key, Action<T> callback)
    {
        this.key = key;
        this.callback = callback;
    }

    public void Cancel()
    {
        callback = null;
    }

    public void OnComplete(T t)
    {
        if (callback != null)
        {
            callback(t);
        }
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
    private Dictionary<string, Bundle> mAssetBundleDic = new Dictionary<string, Bundle>();

    public AssetMode assetMode { get; private set; }
    public bool initialized { get; private set; }

    private AsyncOperation mAsyncOperation;

    private List<string> mBundleNameList = new List<string>();
    private float mLastUnloadTime;
    /// <summary>
    /// 60s自动检测已经销毁的资源，并释放对应的Bundle
    /// </summary>
    public float unloadInterval = 60;

    public void Init()
    {
        if (initialized )
        {
            return;
        }
        StartCoroutine(InitManifest());
    }

    private IEnumerator InitManifest()
    {
#if UNITY_EDITOR
        string path = string.Format("{0}{1}", AssetPath.persistentDataPath, AssetPath.ASSETS_FILE);

        AssetPath.mode = (AssetMode) PlayerPrefs.GetInt("assetMode");
        if (AssetPath.mode == AssetMode.Editor)
        {
            path = string.Format("{0}/{1}", Application.dataPath, AssetPath.ASSETS_FILE);
        }
#else
		string path = string.Format("{0}{1}", AssetPath.persistentDataPath, AssetPath.ASSETS_FILE);
        if (File.Exists(path) == false)
        {
            path = string.Format("{0}{1}", AssetPath.streamingAssetsPath, AssetPath.ASSETS_FILE);
        }
        AssetPath.mode = AssetMode.AssetBundle;

#endif
        assetMode = AssetPath.mode;
        if (mAsyncOperation != null)
        {
            yield return new WaitUntil(() => mAsyncOperation.isDone);
        }
        else
        {
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                mAsyncOperation = request.SendWebRequest();
                yield return mAsyncOperation;

                if (string.IsNullOrEmpty(request.downloadHandler.text) == false)
                {
                    AssetPath.FromXml(request.downloadHandler.text);
                    if (assetMode == AssetMode.AssetBundle)
                    {
                        string manifest = GetPath(AssetPath.manifest);

                        var manifestRequest = AssetBundle.LoadFromFileAsync(manifest);

                        yield return manifestRequest;

                        if (manifestRequest.isDone && manifestRequest.assetBundle)
                        {
                            InitFinish(manifestRequest.assetBundle);
                        }
                        else
                        {
                            Debug.LogError("Load assetbundle:" + manifest + " failed!!");
                        }
                    }
                    else
                    {
                        InitFinish(null);
                    }
                }
                else
                {
                    Debug.LogError(request.error + ":" + request.url);
                }
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

    public LoadTask<Asset<T>> LoadAsset<T>(string key, Action<Asset<T>> callback)where  T:Object
    {
        LoadTask<Asset<T> > task = new LoadTask<Asset<T>>(key,callback);

        StartCoroutine(LoadAsset(task));

        return task;
    }

    private IEnumerator LoadAsset<T>(LoadTask<Asset<T>> task) where T : UnityEngine.Object
    {
        if (initialized == false)
        {
            Init();
            yield return new WaitUntil(() => initialized == true);
        }

        Bundle bundle = CreateBundle(task.bundleName,task.type);

        if (bundle.isLoading)
        {
            yield return new WaitUntil(() => bundle.isDone);
        }
        StartCoroutine(bundle.LoadAsset(task));
    }

    public void LoadScene(string key, LoadSceneMode mode, Action<Scene> callback)
    {
        LoadTask<Scene> task = new LoadTask<Scene>(key, callback);
        StartCoroutine(LoadSceneAsync(task,mode));
    }

    private IEnumerator LoadSceneAsync(LoadTask<Scene> task, LoadSceneMode mode)
    {
        if(initialized== false)
        {
            Init();
            yield return new WaitUntil(() => initialized == true);
        }
        string sceneName = Path.GetFileNameWithoutExtension(task.assetName);
  
        if (assetMode == AssetMode.AssetBundle)
        {
            Bundle bundle = CreateBundle(task.assetName, task.type);
            yield return bundle.LoadBundleAsync();

            var request = SceneManager.LoadSceneAsync(sceneName, mode);
            yield return request;

            task.OnComplete(SceneManager.GetActiveScene());
        }
        else
        {
            var request = SceneManager.LoadSceneAsync(sceneName);
            yield return request;
            task.OnComplete(SceneManager.GetActiveScene());

            yield break;
        }
    }
    public void UnLoadScene(Scene scene,Action callback)
    {
        StartCoroutine(UnloadSceneAsync(scene,callback));
    }

    private IEnumerator UnloadSceneAsync(Scene scene,Action callback)
    {
        var request = SceneManager.UnloadSceneAsync(scene);
        yield return request;
        if (callback != null)
        {
            callback();
        }
    }

    public Bundle GetBundle(string bundleName)
    {
        Bundle bundle = null;

        mAssetBundleDic.TryGetValue(bundleName, out bundle);

        return bundle;
    }

    public Bundle CreateBundle(string bundleName,AssetType assetType)
    {
        Bundle bundle = GetBundle(bundleName);

        if(bundle == null)
        {
            bundle = new Bundle(bundleName, GetAllDependencies(bundleName),assetType);
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
