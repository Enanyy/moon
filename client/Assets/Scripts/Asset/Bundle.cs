using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Bundle
{
    public string bundleName { get; private set; }
    public AssetBundle bundle { get; private set; }
    public AssetType assetType { get; private set; }

    public Dictionary<string, Bundle> dependences { get; private set; }
    public string[] dependenceNames { get; private set; }


    //场景中实例化出来的,即引用
    public Dictionary<string, List<IAsset>> references { get; private set; }
    //已经加载出来的asset
    private Dictionary<string, Object> mAssetDic = new Dictionary<string, Object>();

    //缓存的场景资源
    private Dictionary<string,List<IAsset>> mCacheAssetDic = new Dictionary<string, List<IAsset>>();

    private AsyncOperation mAsyncOperation;

    public Bundle(string bundleName,string[] dependenceNames,AssetType assetType)
    {
        dependences = new Dictionary<string, Bundle>();
        references = new Dictionary<string, List<IAsset>>();

        this.bundleName = bundleName;
        this.dependenceNames = dependenceNames;
        this.assetType = assetType;
    }
    public IEnumerator LoadBundleAsync()
    {
#if UNITY_EDITOR
        if (AssetManager.Instance.assetMode == AssetMode.Editor)
        {
            yield break;
        }
#endif
        if (dependenceNames != null)
        {
            for (int i = 0; i < dependenceNames.Length; ++i)
            {
                string dependenceName = dependenceNames[i];

                if (dependences.ContainsKey(dependenceName) == false)
                {
                    Bundle bundleObject = AssetManager.Instance.CreateBundle(dependenceName,assetType);

                    dependences[dependenceName] = bundleObject;

                    if (bundleObject.bundle == null)
                    {
                        yield return bundleObject.LoadBundleAsync();
                    }
                }
            }
        }

        string path = AssetManager.Instance.GetPath(bundleName);

        mAsyncOperation = AssetBundle.LoadFromFileAsync(path);

        yield return mAsyncOperation;

        var request = mAsyncOperation as AssetBundleCreateRequest;

        if (request.isDone && request.assetBundle)
        {
            bundle = request.assetBundle;
        }
        else
        {
            Debug.LogError("Load assetbundle:" + bundleName + " failed from:" + bundleName + "!!");
        }
    }
    
    public IEnumerator LoadAsset<T>(LoadTask<Asset<T>> task) where T : UnityEngine.Object
    {
        Asset<T> assetObject = null;
        Object asset = null;

        string assetName = task.assetName;
        if (mCacheAssetDic.ContainsKey(assetName) )
        {
            var list = mCacheAssetDic[assetName];
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    assetObject = list[i] as Asset<T>;
                    if (assetObject != null)
                    {
                        list.RemoveAt(i);break;
                    }
                }    
            }
        }

        if (assetObject != null)
        {
            task.OnComplete(assetObject);             
        }
        else
        {
            if (task.type != AssetType.Resource)
            {
#if UNITY_EDITOR
                if (AssetManager.Instance.assetMode == AssetMode.Editor)
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
                {
#endif
                    if (bundle == null)
                    {
                        if (mAsyncOperation == null)
                        {
                            yield return LoadBundleAsync();
                        }
                        else
                        {
                            yield return new WaitUntil(() => mAsyncOperation.isDone);
                        }
                    }

                    if (mAssetDic.ContainsKey(assetName) == false)
                    {
                        if (bundle != null)
                        {
                            var request = bundle.LoadAssetAsync(assetName);

                            yield return request;
                            if (request.asset!=null && mAssetDic.ContainsKey(assetName) == false)
                            {
                                asset = request.asset;
                                mAssetDic.Add(assetName, asset);
                            }
                        }
                    }
#if UNITY_EDITOR
                }
#endif

            }
            else
            {
                if (mAssetDic.ContainsKey(assetName) == false)
                {
                    if (mAsyncOperation == null)
                    {
                        mAsyncOperation = Resources.LoadAsync(assetName);
                    }

                    yield return mAsyncOperation;

                    var request = mAsyncOperation as ResourceRequest;

                    if (request.asset != null && mAssetDic.ContainsKey(assetName) == false)
                    {
                        asset = request.asset;

                        mAssetDic.Add(assetName, asset);
                    }
                }
            }

            if (asset == null)
            {
                mAssetDic.TryGetValue(assetName, out asset);
            }

            if (asset)
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

            task.OnComplete(assetObject);

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
        if (references.ContainsKey(name))
        {
            references[name].Remove(reference);
        }

        if (mCacheAssetDic.ContainsKey(name))
        {
            mCacheAssetDic[name].Remove(reference);
        }

        int referenceCount = 0;
        var it = references.GetEnumerator();
        while(it.MoveNext())
        {
            referenceCount += it.Current.Value.Count;
        }

        if(referenceCount==0)
        {
            UnLoad();
        }
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
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].destroyed)
                {
                    list.RemoveAt(i);
                }
            }

            referenceCount += list.Count;
        }

        if (referenceCount == 0)
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

        if (mCacheAssetDic.ContainsKey(assetObject.assetName) == false)
        {
            mCacheAssetDic.Add(assetObject.assetName,new List<IAsset>());
        }

        var list = mCacheAssetDic[assetObject.assetName];
        if (list.Contains(assetObject) == false)
        {
            list.Add(assetObject);
        }
    }

    public void UnLoad()
    {
        if (AssetManager.Instance.OtherDependence(this, bundleName) == false)
        {
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
            while(dependence.MoveNext())
            {
                dependence.Current.Value.UnLoad();
            }
            dependences.Clear();

            if (assetType == AssetType.Resource)
            {
                var asset = mAssetDic.GetEnumerator();
                while (asset.MoveNext())
                {
                    Resources.UnloadAsset(asset.Current.Value);
                }
            }

            mAssetDic.Clear();
        }
    }
}

