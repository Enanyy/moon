using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BundleObject
{
    public string bundleName { get; private set; }
    public AssetBundle bundle { get; private set; }

    public Dictionary<string, BundleObject> dependences { get; private set; }
    public string[] dependenceNames { get; private set; }
    public List<LoadTask<BundleObject>> onFinished { get; private set; }

    //场景中实例化出来的,即引用
    public Dictionary<string, List<IAssetObject>> references { get; private set; }
    //已经加载出来的asset
    private Dictionary<string, Object> mAssetDic = new Dictionary<string, Object>();

    //缓存的场景资源
    private Dictionary<string,List<IAssetObject>> mCacheAssetDic = new Dictionary<string, List<IAssetObject>>();

    enum BundleType
    {
        AssetBundle,
        Resource
    }

    private BundleType mType = BundleType.AssetBundle;

    public BundleObject(string bundleName,string[] dependenceNames)
    {
        dependences = new Dictionary<string, BundleObject>();
        references = new Dictionary<string, List<IAssetObject>>();
        onFinished = new List<LoadTask<BundleObject>>();
        this.bundleName = bundleName;
        this.dependenceNames = dependenceNames; 
    }

    public void LoadSync()
    {      
        if (dependenceNames != null)
        {
            for (int i =0; i <dependenceNames.Length; ++i)
            {
                string dependenceName = dependenceNames[i];

                if (dependences.ContainsKey(dependenceName) == false)
                {
                    BundleObject bundleObject = AssetManager.Instance.CreateBundle(dependenceName);

                    dependences[dependenceName] = bundleObject;

                    if (bundleObject.bundle == null)
                    {
                        bundleObject.LoadSync();
                    }
                }
            }
        }
        string path = AssetManager.Instance.GetPath(bundleName);

        bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            Debug.LogError("Load assetbundle:" + bundleName + " failed!!");
        }

        Finish();

    }
    public IEnumerator LoadAsync()
    {
        if (dependenceNames != null)
        {
            for (int i = 0; i < dependenceNames.Length; ++i)
            {
                string dependenceName = dependenceNames[i];

                if (dependences.ContainsKey(dependenceName) == false)
                {
                    BundleObject bundleObject = AssetManager.Instance.CreateBundle(dependenceName);

                    dependences[dependenceName] = bundleObject;

                    if (bundleObject.bundle == null)
                    {
                        var coroutine =  AssetManager.Instance.StartCoroutine(bundleObject.LoadAsync());
                        yield return coroutine;
                    }
                }
            }
        }

        string path = AssetManager.Instance.GetPath(bundleName);

        var request = AssetBundle.LoadFromFileAsync(path);
        yield return request;

        if (request.isDone && request.assetBundle)
        {
            bundle = request.assetBundle;
        }
        else
        {
            Debug.LogError("Load assetbundle:" + bundleName + " failed from:" + bundleName + "!!");
        }

        Finish();

    }

    public IEnumerator LoadWWW()
    {
        if (dependenceNames != null)
        {
            for (int i = 0; i < dependenceNames.Length; ++i)
            {
                string dependenceName = dependenceNames[i];

                if (dependences.ContainsKey(dependenceName) == false)
                {
                    BundleObject bundleObject= AssetManager.Instance.CreateBundle(dependenceName);

                    dependences[dependenceName] = bundleObject;

                    if (bundleObject.bundle == null)
                    {
                        var coroutine = AssetManager.Instance.StartCoroutine(bundleObject.LoadWWW());
                        yield return coroutine;
                    }
                }
            }
        }

        string path = AssetManager.Instance.GetPath(bundleName);

        using (WWW www = new WWW(path))
        {
            yield return www;
            if(www.isDone && www.assetBundle)
            {
                bundle = www.assetBundle;
            }
            else
            {
                Debug.LogError("Load assetbundle:" + bundleName + " failed from:" + bundleName + "!!");
            }

            Finish();
        }
    }

    public IEnumerator LoadResource()
    {
        mType = BundleType.Resource;

        var request = Resources.LoadAsync(bundleName);

        yield return request;
        if (request.asset != null)
        {
            mAssetDic.Add(bundleName,request.asset);
        }

        Finish();
    }


    private void Finish()
    {
        int count = 0;
        for (int i = 0; i < onFinished.Count; i++)
        {
            var task = onFinished[i];
            if (task != null && task.callback != null)
            {
                task.callback(this);
                count++;
            }
        }

        if (onFinished.Count > 0 && count == 0)
        {
            UnLoad();
        }
        onFinished.Clear();
      
    }

    public Object LoadAsset(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        if (bundle && mAssetDic.ContainsKey(name) == false)
        {
            if (bundle != null)
            {
                mAssetDic.Add(name, bundle.LoadAsset<Object>(name));
            }
        }

        if (mAssetDic.ContainsKey(name))
        {
            return mAssetDic[name];
        }
        return null;
    }

    public AssetObject<T> LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        AssetObject<T> assetObject = null;
        if (mCacheAssetDic.ContainsKey(assetName) )
        {
            var list = mCacheAssetDic[assetName];
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    assetObject = list[i] as AssetObject<T>;
                    if (assetObject != null)
                    {
                        list.RemoveAt(i);break;
                    }
                }    
            }
        }

        if (assetObject == null)
        {
            var asset = LoadAsset(assetName);
            if (asset)
            {
                if (typeof(T) == typeof(GameObject))
                {
                    var go = UnityEngine.Object.Instantiate(asset) as GameObject;

                    assetObject = new AssetObject<T>(assetName, this, asset, go as T);
                }
                else
                {
                    assetObject = new AssetObject<T>(assetName, this, asset, asset as T);
                }
            }
        }

        return assetObject;
    }

    public void AddReference(IAssetObject reference)
    {
        if(reference== null)
        {
            return;
        }
        string name = reference.assetName;
        if(references.ContainsKey(name)==false)
        {
            references.Add(name, new List<IAssetObject>());
        }
        if(references[name].Contains(reference)==false)
        {
            references[name].Add(reference);
        }
    }

    public void RemoveReference(IAssetObject reference)
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

    public void ReturnAsset(IAssetObject assetObject)
    {
        if (assetObject == null)
        {
            return;
        }

        if (mCacheAssetDic.ContainsKey(assetObject.assetName) == false)
        {
            mCacheAssetDic.Add(assetObject.assetName,new List<IAssetObject>());
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

            var asset = mAssetDic.GetEnumerator();
            while (asset.MoveNext())
            {
                if (mType == BundleType.Resource)
                {
                    Resources.UnloadAsset(asset.Current.Value);
                }
            }
            mAssetDic.Clear();
        }
    }
}

