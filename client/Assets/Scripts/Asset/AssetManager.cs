using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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

    private Asset<AssetBundleManifest> mAssetManifest;

    private Dictionary<string, Bundle> mAssetBundleDic = new Dictionary<string, Bundle>();

    public LoadStatus status { get; private set; } = LoadStatus.None;

    private List<string> mBundleNameList = new List<string>();
    private float mLastUnloadTime;
    /// <summary>
    /// 60s自动检测已经销毁的资源，并释放对应的Bundle
    /// </summary>
    public float unloadInterval = 60;

    private List<ISceneLoadTask> mSceneLoadTasks = new List<ISceneLoadTask>();

    
    private IEnumerator BeginInitialize()
    {     
        if (status == LoadStatus.Loading)
        {
            yield return new WaitUntil(() => status == LoadStatus.Done);
        }
        else
        {
            string assetFile = AssetPath.GetAssetFile();

            Debug.Log("AssetMode:" + AssetPath.mode.ToString());

            using (UnityWebRequest request = UnityWebRequest.Get(assetFile))
            {
                status = LoadStatus.Loading;

                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                yield return operation;

                if (string.IsNullOrEmpty(request.downloadHandler.text) == false)
                {
                    //解析资源列表
                    AssetPath.FromXml(request.downloadHandler.text);

                    if (AssetPath.mode == AssetMode.AssetBundle)
                    {
                        Bundle bundle = GetOrCreateBundle(AssetPath.manifest);

                        AssetCustomLoadTask<AssetBundleManifest> task = new AssetCustomLoadTask<AssetBundleManifest>(AssetPath.manifest, "AssetBundleManifest", FinishInitialize);

                        yield return bundle.LoadAsset(task);
 
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
    private void FinishInitialize(Asset<AssetBundleManifest> asset)
    {
        mAssetManifest = asset;

        if(mAssetManifest!= null && mAssetManifest.assetObject!= null)
        {
            DontDestroyOnLoad(mAssetManifest.assetObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;

        status = LoadStatus.Done;

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
    /// key可以是文件名(带后缀)或者文件路径(Assets/...)。
    /// 注意，当非实例化资源（非GameObject，如Material，Texture，TextAsset等）使用完毕后,需要主动调用Destroy去释放资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public AssetLoadTask<T> LoadAsset<T>(string key, Action<Asset<T>> callback)where  T: UnityEngine.Object
    {
        AssetLoadTask<T > task = new AssetLoadTask<T>(key.ToLower(),callback);
        
        LoadAsset(task);

        return task;
    }
    /// <summary>
    /// 自定义LoadTask。当非实例化资源（非GameObject，如Material，Texture，TextAsset等）使用完毕后,需要主动调用Destroy去释放资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    public void LoadAsset<T>(IAssetLoadTask<T> task) where T : UnityEngine.Object
    {
        if(task== null)
        {
            return;
        }
        
        StartCoroutine(LoadAssetAsync(task));
    }

    private IEnumerator LoadAssetAsync<T>(IAssetLoadTask<T> task) where T : UnityEngine.Object
    {
        if (status != LoadStatus.Done)
        {
            yield return BeginInitialize();
        }
       

        Bundle bundle = GetOrCreateBundle(task.bundleName);

        yield return bundle.LoadAsset(task);
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="key"></param>
    /// <param name="mode"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public SceneLoadTask LoadScene(string key, LoadSceneMode mode, Action<Scene,LoadSceneMode> callback)
    {
        SceneLoadTask task = new SceneLoadTask(key.ToLower(), mode, callback);
        LoadScene(task);
        return task;
    }
    /// <summary>
    /// 自定义LoadTask
    /// </summary>
    /// <param name="task"></param>
    public void LoadScene(ISceneLoadTask task)
    {
        if(task == null)
        {
            return;
        }
        StartCoroutine(LoadSceneAsync(task));
    }

    private IEnumerator LoadSceneAsync(ISceneLoadTask task)
    {
        if (status != LoadStatus.Done)
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for(int i =0; i < mSceneLoadTasks.Count;)
        {
            var task = mSceneLoadTasks[i];
            if(task.isCancel)
            {
                if(task.sceneName == scene.name && mode == LoadSceneMode.Additive)
                {
                    UnLoadScene(scene, null);
                }
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
            bundle = new Bundle(bundleName);
            mAssetBundleDic.Add(bundleName, bundle);
        }
        return bundle;
    }

    public void RemoveBundle(Bundle bundle)
    {
        if (bundle == null || string.IsNullOrEmpty(bundle.bundleName))
        {
            return;
        }

        mAssetBundleDic.Remove(bundle.bundleName);
    }

   
    /// <summary>
    /// 获取直接依赖
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public string[] GetDirectDependencies(string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName) || mAssetManifest == null || mAssetManifest.assetObject == null)
        {
            return null;
        }

        return mAssetManifest.assetObject.GetDirectDependencies(bundleName);
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
        mAssetManifest = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
