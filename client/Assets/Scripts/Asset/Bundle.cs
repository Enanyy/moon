using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum BundleStatus
{
    None,
    Loading,
    Done,
}

public class Bundle
{
    public string bundleName { get; private set; }
    public AssetBundle bundle { get; private set; }
    public string[] dependenceNames { get; private set; }

    /// <summary>
    /// 依赖哪些资源？
    /// </summary>
    public Dictionary<string, Bundle> dependences { get; private set; }
    /// <summary>
    /// 场景中实例化出来的,即引用
    /// </summary>
    public Dictionary<string, List<IAsset>> references { get; private set; }
    //已经加载出来的asset
    private Dictionary<string, Object> mAssetDic = new Dictionary<string, Object>();

    //缓存的场景资源
    private Dictionary<string,List<IAsset>> mCacheAssetDic = new Dictionary<string, List<IAsset>>();

    /// <summary>
    /// Resource资源名
    /// </summary>
    private HashSet<string> mResourceAssets = new HashSet<string>();

    private AsyncOperation mAsyncOperation;
  
    public BundleStatus status { get; private set; }

    private int mAssetCount = 0;

    /// <summary>
    /// 该Bundle所有资源数
    /// </summary>
    public int assetCount
    {
        get
        {
            if (mAssetCount == 0)
            {
                if (bundle != null)
                {
                    mAssetCount = bundle.GetAllAssetNames().Length;
                }
            }
            return mAssetCount;
        }
    }

    private HashSet<ILoadTask> mLoadTasks = new HashSet<ILoadTask>();

    public Bundle(string bundleName,string[] dependenceNames)
    {
        dependences = new Dictionary<string, Bundle>();
        references = new Dictionary<string, List<IAsset>>();

        this.bundleName = bundleName;
        this.dependenceNames = dependenceNames;

        status = BundleStatus.None;
    }
    public IEnumerator LoadBundleAsync()
    {
#if UNITY_EDITOR
        if (AssetPath.mode == AssetMode.Editor)
        {
            yield break;
        }
#endif
        if (dependenceNames != null)
        {
            for (int i = 0; i < dependenceNames.Length; ++i)
            {
                string dependenceName = dependenceNames[i];

                Bundle bundleObject;
                if (dependences.TryGetValue(dependenceName, out bundleObject) == false)
                {
                    bundleObject = AssetManager.Instance.GetOrCreateBundle(dependenceName);

                    dependences.Add(dependenceName, bundleObject);
                }
            }
        }

        var it = dependences.GetEnumerator();
        while (it.MoveNext())
        {
            Bundle bundleObject = it.Current.Value;
            if (bundleObject.status ==  BundleStatus.None)
            {
                yield return bundleObject.LoadBundleAsync();
            }
        }

        if (mAsyncOperation == null)
        {
            string path = AssetPath.GetFullPath(bundleName);
            mAsyncOperation = AssetBundle.LoadFromFileAsync(path);

            status = BundleStatus.Loading;
        }

        yield return mAsyncOperation;

        status = BundleStatus.Done;

        if (bundle == null)
        {
            var request = mAsyncOperation as AssetBundleCreateRequest;

            if (request.isDone && request.assetBundle)
            {
                bundle = request.assetBundle;
            }
            else
            {
                mAsyncOperation = null;
                //Debug.Log("Load assetbundle:" + bundleName + " failed from:" + bundleName + "!!");
            }
        }
  }

  public IEnumerator LoadAsset<T>(IAssetLoadTask<T> task) where T : UnityEngine.Object
    {
        if (task == null || task.isCancel)
        {
            yield break;
        }

        Asset<T> assetObject = null;
        Object asset = null;
      
        string assetName = task.assetName;
        
        if (mCacheAssetDic.TryGetValue(assetName,out List<IAsset> list))
        {
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    assetObject = list[i] as Asset<T>;
                    if (assetObject != null)
                    {
                        list.RemoveAt(i); break;
                    }
                }
            }
        }
        

        if (assetObject != null)
        {
            task.OnCompleted(assetObject);
        }
        else
        {

#if UNITY_EDITOR
            if (AssetPath.mode == AssetMode.Editor)
            {
                mAssetDic.TryGetValue(assetName, out asset);

                if (asset == null)
                {
                    asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
                    if (asset)
                    {
                        mAssetDic.Add(assetName, asset);
                    }
                }
            }
            else
#endif
            {

                mLoadTasks.Add(task);

                //不是Resources资源

                if (bundle == null)
                {
                    if (mAsyncOperation == null && task.isCancel == false)
                    {
                        yield return LoadBundleAsync();
                    }
                    else
                    {
                        yield return new WaitUntil(() => mAsyncOperation.isDone || task.isCancel);
                    }
                }

                if (mAssetDic.ContainsKey(assetName) == false)
                {
                    if (bundle != null && task.isCancel == false)
                    {
                        var request = bundle.LoadAssetAsync(assetName);

                        yield return request;
                        if (request.asset != null && mAssetDic.ContainsKey(assetName) == false)
                        {
                            asset = request.asset;
                            mAssetDic.Add(assetName, asset);
                        }
                    }
                }
            }

            if (mAssetDic.ContainsKey(assetName) == false && task.isCancel == false)
            {
                ///尝试从Resources加载

                string path = AssetPath.GetResourcePath(assetName);

                mAsyncOperation = Resources.LoadAsync(path);


                yield return mAsyncOperation;

                var request = mAsyncOperation as ResourceRequest;

                if (request.asset != null && mAssetDic.ContainsKey(assetName) == false)
                {
                    asset = request.asset;

                    mAssetDic.Add(assetName, asset);
                    mResourceAssets.Add(assetName);
                }
            }


            if (asset == null)
            {
                mAssetDic.TryGetValue(assetName, out asset);
            }

            if (asset && task.isCancel == false)
            {
                if (typeof(T) == typeof(GameObject))
                {
                    var go = UnityEngine.Object.Instantiate(asset) as GameObject;

                    assetObject = new Asset<T>(assetName, this, asset, go as T);
                }
                else
                {
                    assetObject = new Asset<T>(assetName, this, asset, asset as T);
                }
            }

            mLoadTasks.Remove(task);

            if (task.isCancel == false)
            {
                task.OnCompleted(assetObject);
            }
            else
            {
                RemoveReference();
            }
        }
    }

    public void AddReference(IAsset reference)
    {
        if(reference== null)
        {
            return;
        }
        string name = reference.assetName;
        if(references.ContainsKey(name)==false)
        {
            references.Add(name, new List<IAsset>());
        }
        if(references[name].Contains(reference)==false)
        {
            references[name].Add(reference);
        }
    }

    public void RemoveReference(IAsset reference)
    {
        if (reference == null)
        {
            return;
        }
        string name = reference.assetName;
        List<IAsset> list;
        if (references.TryGetValue(name, out list))
        {
            list.Remove(reference);
        }

        if (mCacheAssetDic.TryGetValue(name, out list))
        {
            list.Remove(reference);
        }

        RemoveReference();
    }
    /// <summary>
    /// 移除已经destroy的资源引用
    /// </summary>
    public void RemoveReference()
    {
        int referenceCount = 0;
        var it = references.GetEnumerator();
        while (it.MoveNext())
        {
            var list = it.Current.Value;
            for (int i = 0; i < list.Count;)
            {
                if (list[i] == null || list[i].destroyed)
                {
                    list.RemoveAt(i); continue;
                }
                referenceCount++;
                ++i;
            }
        }
        int loadtaskCount = 0;
        var loadtask = mLoadTasks.GetEnumerator();
        while (loadtask.MoveNext())
        {
            if (loadtask.Current.isCancel == false)
            {
                loadtaskCount++;
            }
        }
        if (referenceCount == 0 && loadtaskCount ==0)
        {
            UnLoad();
        }

    }

    public bool Dependence(string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName))
        {
            return false;
        }

        if(dependenceNames!=null)
        {
            for (int i = 0; i < dependenceNames.Length; ++i)
            {
                string dependenceName = dependenceNames[i];
                if(dependenceName == bundleName)
                {
                    return true;
                }
            }
        }

        var it = dependences.GetEnumerator();

        while(it.MoveNext())
        {
            if(it.Current.Value.Dependence(bundleName))
            {
                return true;
            }
        }
  

        return false;
    }

    public void ReturnAsset(IAsset assetObject)
    {
        if (assetObject == null)
        {
            return;
        }

        List<IAsset> list;
        if (mCacheAssetDic.TryGetValue(assetObject.assetName, out list) == false)
        {
            list = new List<IAsset>();
            mCacheAssetDic.Add(assetObject.assetName, list);
        }

        if (list.Contains(assetObject) == false)
        {
            list.Add(assetObject);
        }
    }

    public void UnLoad()
    {
        if (AssetManager.Instance.OtherDependence(this, bundleName) == false)
        {
            Debug.Log("卸载Bundle:" + bundleName);

            AssetManager.Instance.RemoveBundle(this);

            if (bundle != null)
            {
                bundle.Unload(true);
                bundle = null;
            }

            var reference = references.GetEnumerator();
            while (reference.MoveNext())
            {
                for (int i = 0; i < reference.Current.Value.Count; i++)
                {
                    reference.Current.Value[i].Destroy(false);
                }
                reference.Current.Value.Clear();
            }
            references.Clear();

            var dependence = dependences.GetEnumerator();
            while (dependence.MoveNext())
            {
                var dependenceBundle = dependence.Current.Value;
                if (string.IsNullOrEmpty(dependenceBundle.bundleName) == false)
                {
                    dependenceBundle.UnLoad();
                }
            }
            dependences.Clear();


            var asset = mAssetDic.GetEnumerator();
            while (asset.MoveNext())
            {
                if (mResourceAssets.Contains(asset.Current.Key))
                {
                    Resources.UnloadAsset(asset.Current.Value);
                }
            }


            mAssetDic.Clear();
            mCacheAssetDic.Clear();
            mResourceAssets.Clear();

            bundleName = null;
            mAsyncOperation = null;
            dependenceNames = null;
        }
    }
}

