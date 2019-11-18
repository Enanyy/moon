using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;


public class AssetPath
{
    public string name;
    public string group;
    public string path;
    public long size;
    public string md5;

    /// <summary>
    /// 是否为Resources资源
    /// </summary>
    public bool isRecource { get { return path.Contains("resources/"); } }
   
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
        if (string.IsNullOrEmpty(group) == false)
        {
            AddAttribute(doc, node, "group", group);
        }

        AddAttribute(doc, node, "path", path);
        AddAttribute(doc, node, "size", size.ToString());
        AddAttribute(doc, node, "md5", md5);
      
    }
    private void AddAttribute(XmlDocument doc,XmlElement node,string name, string value)
    {
        XmlAttribute type = doc.CreateAttribute(name);
        type.Value = value;
        node.Attributes.Append(type);
    }

    public void FromXml(XmlElement element)
    {
        name = element.GetAttribute("name");
        group = element.GetAttribute("group");
        path = element.GetAttribute("path");
        size = element.GetAttribute("size").ToInt64Ex();
        md5 = element.GetAttribute("md5");
     
    }

    #region Static

    public static string manifest;
    public static int version;
    public static Dictionary<string, AssetPath> assets = new Dictionary<string, AssetPath>();

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
    public const string ASSETSFILE = "assets.txt";

    public static void AddAsset(AssetPath asset)
    {
        if(asset == null)
        {
            return;
        }
        if(assets.ContainsKey(asset.name) == false)
        {
            assets.Add(asset.name, asset);
        }
        else
        {
            assets[asset.name]= asset;
        }

        if (assets.ContainsKey(asset.path) == false)
        {
            assets.Add(asset.path, asset);
        }
        else
        {
            assets[asset.path] = asset;
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
    public static void RemoveAsset(AssetPath asset)
    {
        if(asset== null)
        {
            return;
        }
        assets.Remove(asset.name);
        assets.Remove(asset.path);
#if UNITY_EDITOR
        assets.Remove(asset.md5);
#endif
    }

    private static string FormatRootPath(string path)
    {
#if UNITY_EDITOR
        if (mode == AssetMode.Editor)
        {
            return  Application.dataPath + "/";
        }
        else
#endif
        {
#if UNITY_ANDROID
            return string.Format("{0}/r/", path);
#elif UNITY_IOS
            return string.Format("{0}/r/", path);
#elif UNITY_EDITOR_OSX
            return string.Format("file://{0}/../r/{1}/", Application.dataPath,UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#elif UNITY_STANDALONE_OSX
            return string.Format("file://{0}/../r/StandaloneOSX/", Application.dataPath);
#elif UNITY_EDITOR_WIN
            return string.Format("{0}/../r/{1}/", Application.dataPath,UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#elif UNITY_STANDALONE_WIN
            if (IntPtr.Size == 8) //64 bit
            {
                return string.Format("{0}/../r/StandaloneWindows64/", Application.dataPath);
            }
            else  //32 bit
            {
                return string.Format("{0}/../r/StandaloneWindows/", Application.dataPath);
            }
#else
            return path;
#endif
        }
    }

    private static string mStreamingAssetsPath;
    /// <summary>
    /// 结尾带/
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
    /// 结尾带/
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
    public static void FromXml(string xml)
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
                AssetPath asset = new AssetPath();
                asset.FromXml(child);
                AddAsset(asset);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }
    } 

    public static string ToXml()
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
        HashSet<AssetPath> set = new HashSet<AssetPath>();
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
    public static AssetPath Get(string key)
    {
        AssetPath asset;
        assets.TryGetValue(key, out asset);
        return asset;
    }

    public static string GetFullPath(string path)
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
        else
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
        assets.Clear();
    }
#endregion
}

