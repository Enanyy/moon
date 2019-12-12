using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public enum AssetMode
{
    Editor,     //����ģʽ
    AssetBundle,//AssetBunldeģʽ
}

/// <summary>
/// ��Դ�ļ�
/// </summary>
public class AssetFile
{
    /// <summary>
    /// ��Դkey
    /// </summary>
    public string name;
    /// <summary>
    /// ��Դ���ڵ�bundle
    /// </summary>
    public string bundle;
    /// <summary>
    /// ��Դ�����������Ҳ��bundle����
    /// </summary>
    public string asset;
    /// <summary>
    /// bundle��С
    /// </summary>
    public long size;
    /// <summary>
    /// bundle��md5,Editor������Դ��InstanceID
    /// </summary>
    public string md5;

    public void ToXml(XmlNode parent)
    {
        XmlDocument doc;
        if (parent.ParentNode == null)
        {
            doc = (XmlDocument)parent;
        }
        else
        {
            doc = parent.OwnerDocument;
        }
        XmlElement node = doc.CreateElement("asset");
        parent.AppendChild(node);

        AddAttribute(doc, node, "name", name);

        if (string.IsNullOrEmpty(asset) == false)
        {
            AddAttribute(doc, node, "asset", asset);
        }

        if (string.IsNullOrEmpty(bundle) == false)
        {
            AddAttribute(doc, node, "bundle", bundle);
        }
        if (size > 0)
        {
            AddAttribute(doc, node, "size", size.ToString());
        }
        if (string.IsNullOrEmpty(md5) == false)
        {
            AddAttribute(doc, node, "md5", md5);
        }
    }
    private void AddAttribute(XmlDocument doc, XmlElement node, string name, string value)
    {
        XmlAttribute type = doc.CreateAttribute(name);
        type.Value = value;
        node.Attributes.Append(type);
    }

    public void FromXml(XmlElement element)
    {
        name = element.GetAttribute("name");
        bundle = element.GetAttribute("bundle");
        asset = element.GetAttribute("asset");
        size = element.GetAttribute("size").ToInt64Ex();
        md5 = element.GetAttribute("md5");
    }
}
/// <summary>
/// ��Դ�б�
/// </summary>
public class AssetList
{
    public string manifest;
    public int version;
    public Dictionary<string, AssetFile> assets = new Dictionary<string, AssetFile>();

    public void Add(AssetFile asset)
    {
        if (asset == null)
        {
            return;
        }
        if (assets.ContainsKey(asset.name) == false)
        {
            assets.Add(asset.name, asset);
        }
        else
        {
            assets[asset.name] = asset;
        }

        if (assets.ContainsKey(asset.asset) == false)
        {
            assets.Add(asset.asset, asset);
        }
        else
        {
            assets[asset.asset] = asset;
        }
#if UNITY_EDITOR
        if (assets.ContainsKey(asset.md5) == false)
        {
            assets.Add(asset.md5, asset);
        }
        else
        {
            assets[asset.md5] = asset;
        }
#endif

    }
    public void Remove(AssetFile asset)
    {
        if (asset == null)
        {
            return;
        }
        assets.Remove(asset.name);
        assets.Remove(asset.asset);
#if UNITY_EDITOR
        assets.Remove(asset.md5);
#endif
    }

    public bool Contains(string key)
    {
        return assets.ContainsKey(key);
    }

    public AssetFile Get(string key)
    {
        assets.TryGetValue(key, out AssetFile file);
        return file;
    }

    public void FromXml(string xml)
    {
        try
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(xml);

            XmlElement root = doc.DocumentElement;

            manifest = root.GetAttribute("manifest");
            version = root.GetAttribute("version").ToInt32Ex();

            assets.Clear();

            for (int i = 0; i < root.ChildNodes.Count; ++i)
            {
                XmlElement child = root.ChildNodes[i] as XmlElement;
                AssetFile asset = new AssetFile();
                asset.FromXml(child);
                Add(asset);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }
    }

    public string ToXml()
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        doc.InsertBefore(dec, doc.DocumentElement);

        XmlElement root = doc.CreateElement("Assets");
        if (string.IsNullOrEmpty(manifest) == false)
        {
            XmlAttribute attribute = doc.CreateAttribute("manifest");
            attribute.Value = manifest;
            root.Attributes.Append(attribute);
        }

        XmlAttribute ver = doc.CreateAttribute("version");
        ver.Value = version.ToString();
        root.Attributes.Append(ver);

        doc.AppendChild(root);
        HashSet<AssetFile> set = new HashSet<AssetFile>();
        var it = assets.GetEnumerator();
        while (it.MoveNext())
        {
            if (set.Contains(it.Current.Value) == false)
            {
                it.Current.Value.ToXml(root);
                set.Add(it.Current.Value);
            }
        }

        MemoryStream ms = new MemoryStream();
        XmlTextWriter xw = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
        xw.Formatting = Formatting.Indented;
        doc.Save(xw);

        ms = (MemoryStream)xw.BaseStream;
        byte[] bytes = ms.ToArray();
        string xml = System.Text.Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

        return xml;
    }

    public void Clear()
    {
        manifest = null;
        assets.Clear();
    }

}

public static class AssetPath
{

    public readonly static AssetList list = new AssetList();
  
    public static AssetMode mode 
    {
        get
        {
#if UNITY_EDITOR
             return (AssetMode)PlayerPrefs.GetInt("assetMode");

#else
             return AssetMode.AssetBundle;
#endif
        }
    }
    public static LoadStatus status { get; private set; } = LoadStatus.None;

    public const string ASSETSFILE = "assets.txt";

    public static IEnumerator Initialize()
    {
        if (status == LoadStatus.Done)
        {
            yield break;
        }
        else if (status == LoadStatus.Loading)
        {
            yield return new WaitUntil(() => status == LoadStatus.Done);
        }
        else
        {
            string assetFile = GetAssetFile();

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            //������ƽ̨��UnityWebRequest����Ҫ��file://
            assetFile = string.Format("file://{0}", assetFile);
#endif
            Debug.Log("AssetMode:" + mode.ToString() + " assetFile:" + assetFile);

            using (UnityWebRequest request = UnityWebRequest.Get(assetFile))
            {
                status = LoadStatus.Loading;

                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                yield return operation;

                if (string.IsNullOrEmpty(request.downloadHandler.text) == false)
                {
                    list.FromXml(request.downloadHandler.text);

                    status = LoadStatus.Done;
                }
            }
        }
    }



    private static string FormatRootPath(string path)
    {

#if UNITY_EDITOR
        if (mode == AssetMode.Editor)
        {
            return Application.dataPath + "/";
        }
        else
        {
            return string.Format("{0}/../r/{1}/", Application.dataPath, UnityEditor.EditorUserBuildSettings.activeBuildTarget);
        }
#else
#if UNITY_ANDROID
        return string.Format("{0}/r/", path);
#elif UNITY_IOS
        return string.Format("{0}/r/", path);
#elif UNITY_STANDALONE_WIN
        if (IntPtr.Size == 8) //64 bit
        {
            return string.Format("{0}/../r/StandaloneWindows64/", Application.dataPath);
        }
        else  //32 bit
        {
            return string.Format("{0}/../r/StandaloneWindows/", Application.dataPath);
        }
#elif UNITY_STANDALONE_OSX
            return string.Format("{0}/../r/StandaloneOSX/", Application.dataPath);
#else
        return path;
#endif
#endif

        }

    private static string mStreamingAssetsPath;
    /// <summary>
    /// ��β��/
    /// </summary>
    public static string streamingAssetsPath
    {
        get
        {
            if(string.IsNullOrEmpty(mStreamingAssetsPath))
            {
                mStreamingAssetsPath= FormatRootPath(Application.streamingAssetsPath);
            }

            return mStreamingAssetsPath;
        }
    }

    private static string mPersistentDataPath;

    /// <summary>
    /// ��β��/
    /// </summary>
    public static string persistentDataPath
    {
        get
        {
            if (string.IsNullOrEmpty(mPersistentDataPath))
            {
                mPersistentDataPath = FormatRootPath(Application.persistentDataPath);
            }
            return mPersistentDataPath;
        }
    }

    public static AssetFile Get(string key)
    {
        AssetFile file = list.Get(key);
       
        return file;
    }

    public static string GetFullPath(string path)
    {
        path = string.Format("{0}{1}", persistentDataPath, path);
        if (File.Exists(path) == false)
        {
            path = string.Format("{0}{1}", streamingAssetsPath, path);
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            path = Uri.EscapeUriString(path);
        }
        return path;

    }
    public static string GetResourcePath(string path)
    {
        const string resources = "resources/";

        if (path.Contains(resources))
        {
            path = path.Substring(path.LastIndexOf(resources) + resources.Length);

            if (path.Contains("."))
            {
                path = path.Substring(0, path.LastIndexOf('.'));
            }
            return path;
        }
        return path;
    }
    public static string GetAssetFile()
    {
        string path = string.Format("{0}{1}", persistentDataPath, ASSETSFILE);
        if (File.Exists(path) == false)
        {
            path = string.Format("{0}{1}", streamingAssetsPath, ASSETSFILE);
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            path = Uri.EscapeUriString(path);
        }
        return path;
    }
   
    public static void Clear()
    {
        list.Clear();
    }
}

