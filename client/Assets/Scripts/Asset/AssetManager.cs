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

    public bool initialized { get; private set; }

    private AsyncOperation mAsyncOperation;

    private List<string> mBundleNameList = new List<string>();
    private float mLastUnloadTime;
    /// <summary>
    /// 60s自动检测已经销毁的资源，并释放对应的Bundle
    /// </summary>
    public float unloadInterval = 60;

    private List<SceneLoadTask> mSceneLoadTasks = new List<SceneLoadTask>();

    public void Initialize()
    {
        if (initialized )
        {
            return;
        }
        StartCoroutine(BeginInitialize());
    }

    private IEnumerator BeginInitialize()
    {     
        if (mAsyncOperation != null)
        {
            yield return new WaitUntil(() => mAsyncOperation.isDone);
        }
        else
        {
            string assetFile = AssetPath.GetAssetFile();

            Debug.Log("AssetMode:" + AssetPath.mode.ToString());

            using (UnityWebRequest request = UnityWebRequest.Get(assetFile))
            {
                mAsyncOperation = request.SendWebRequest();
                yield return mAsyncOperation;

                if (string.IsNullOrEmpty(request.downloadHandler.text) == false)
                {
                    AssetPath.FromXml(request.downloadHandler.text);
                    if (AssetPath.mode == AssetMode.AssetBundle)
                    {
                        string manifest = AssetPath.GetFullPath(AssetPath.manifest);

                        var manifestRequest = AssetBundle.LoadFromFileAsync(manifest);

                        yield return manifestRequest;

                        if (manifestRequest.isDone && manifestRequest.assetBundle)
                        {
                            FinishInitialize(manifestRequest.assetBundle);
                        }
                        else
                        {
                            Debug.LogError("Load assetbundle:" + manifest + " failed!!");
                        }
                    }
                    else
                    {
                        FinishInitialize(null);
                    }
                }
                else
                {
                    Debug.LogError(request.error + ":" + request.url);
                }
            }
        }
    }

    private void FinishInitialize(AssetBundle assetBundle)
    {
        if (assetBundle != null)
        {
            mManifestBundle = assetBundle;

            mManifest = mManifestBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

            DontDestroyOnLoad(mManifest);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        initialized = true;
        Debug.Log("Initialize AssetManager Finish!");
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
    /// <summary>
    /// key可以是文件名(带后缀)或者文件路径(Assets/...)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public AssetLoadTask<Asset<T>> LoadAsset<T>(string key, Action<Asset<T>> callback)where  T:Object
    {
        AssetLoadTask<Asset<T> > task = new AssetLoadTask<Asset<T>>(key.ToLower(),callback);

        StartCoroutine(LoadAssetAsync(task));

        return task;
    }

    private IEnumerator LoadAssetAsync<T>(AssetLoadTask<Asset<T>> task) where T : UnityEngine.Object
    {
        if (initialized == false)
        {
            Initialize();
            yield return new WaitUntil(() => initialized);
        }

        Bundle bundle = GetOrCreateBundle(task.bundleName);

        StartCoroutine(bundle.LoadAsset(task));
    }

    public SceneLoadTask LoadScene(string key, LoadSceneMode mode, Action<Scene,LoadSceneMode> callback)
    {
        SceneLoadTask task = new SceneLoadTask(key.ToLower(), mode, callback);
        StartCoroutine(LoadSceneAsync(task));
        return task;
    }

    private IEnumerator LoadSceneAsync(SceneLoadTask task)
    {
        if(initialized== false)
        {
            Initialize();
            yield return new WaitUntil(() => initialized);
        }

        mSceneLoadTasks.Add(task);

        if (AssetPath.mode == AssetMode.AssetBundle)
        {
            Bundle bundle = GetOrCreateBundle(task.assetName);
            yield return bundle.LoadBundleAsync();

            var request = SceneManager.LoadSceneAsync(task.sceneName, task.mode);
            yield return request;

        }
        else
        {
            var request = SceneManager.LoadSceneAsync(task.sceneName);
            yield return request;

            yield break;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for(int i =0; i < mSceneLoadTasks.Count;)
        {
            var task = mSceneLoadTasks[i];
            if(task.isCancel)
            {
                mSceneLoadTasks.RemoveAt(i);
                continue;
            }
            if(task.sceneName == scene.name)
            {
                task.OnCompleted(scene, mode);
                mSceneLoadTasks.RemoveAt(i);
                continue;
            }
            i++;
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
        mAssetBundleDic.TryGetValue(bundleName, out Bundle bundle);

        return bundle;
    }

    public Bundle GetOrCreateBundle(string bundleName)
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
       
         mAssetBundleDic.Remove(bundle.bundleName);
        
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
