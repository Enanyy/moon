using UnityEngine;
public interface IAsset
{
    string assetName { get; }
    Bundle bundle { get; }
    Object asset { get; }
    /// <summary>
    /// 已经被销毁
    /// </summary>
    bool destroyed { get; }


    void Destroy(bool removeReference = true);

}
public class Asset<T>: IAsset where T:Object
{
    public Bundle bundle { get; private set; }

    public string assetName { get; private set; }

    public Object asset { get; private set; }

    public T assetObject { get; private set; }

    public bool destroyed{ get { return assetObject == null; } }

    public Asset(string assetName, Bundle bundle, Object asset, T assetObject)
    {
        this.assetName = assetName;
        this.bundle = bundle;
        this.asset = asset;
        this.assetObject = assetObject;

        if(bundle!= null)
        {
            bundle.AddReference(this);
        }
    }
    public virtual void Destroy(bool removeReference = true)
    { 
        if (typeof(T) == typeof(GameObject))
        {
            Object.Destroy(assetObject as GameObject);
        }

        if (bundle != null && removeReference)
        {
            bundle.RemoveReference(this);
        }
        asset = null;
        assetObject = null;
    }

    public void ReturnAsset()
    {
        if (bundle != null)
        {
            bundle.ReturnAsset(this);
        }
        else
        {
            Destroy();
        }
    }
}

