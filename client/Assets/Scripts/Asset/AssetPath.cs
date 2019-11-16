using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using UnityEngine;
public enum AssetType
{
    Resource,
    StreamingAsset,
    PersistentAsset,
}

public class AssetPath
{
    public string name;
    public string group;
    public string path;
    public long size;
    public string md5;
    public AssetType type = AssetType.Resource;

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
        AddAttribute(doc, node, "type", ((int)type).ToString());

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
        type = (AssetType)element.GetAttribute("type").ToInt32Ex();
    }

    #region Static

    public static string manifest;
    public static int version;
    public static Dictionary<string, AssetPath> assets = new Dictionary<string, AssetPath>();

    public static AssetMode mode = AssetMode.AssetBundle;
    public const string ASSETS_FILE = "assets.txt";

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
#if UNITY_EDITOR
        if (assets.ContainsKey(asset.path) == false)
        {
            assets.Add(asset.path, asset);
        }
        else
        {
            assets[asset.path] = asset;
        }
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
#if UNITY_EDITOR
        assets.Remove(asset.path);
        assets.Remove(asset.md5);
#endif
    }

    private static string FormatPath(string path)
    {
#if UNITY_EDITOR || UNITY_EDITOR_OSX
        if (mode != AssetMode.AssetBundle)
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
            return string.Format("file://{0}/../r/{1}/", Application.dataPath,
                    UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#elif UNITY_EDITOR
            return string.Format("{0}/../r/{1}/", Application.dataPath,
            UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
            return path;
#endif
        }
    }

    /// <summary>
    /// 结尾带/
    /// </summary>
    public static string streamingAssetsPath
    {
        get
        {
            return FormatPath(Application.streamingAssetsPath);
        }
    }

    /// <summary>
    /// 结尾带/
    /// </summary>
    public static string persistentDataPath
    {
        get
        {
            return FormatPath(Application.persistentDataPath);
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
        catch (Exception ex)
        {
            throw ex;
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
    public static AssetPath Get(string name)
    {
        AssetPath asset;
        assets.TryGetValue(name, out asset);
        return asset;
    }

    public static string GetPath(string name)
    {
        string path = name;
        AssetPath asset = Get(name);
        if (asset != null)
        {
            switch (asset.type)
            {
                case AssetType.StreamingAsset:
                    {
                        path = string.Format("{0}{1}", streamingAssetsPath, asset.path);
                    }
                    break;
                case AssetType.PersistentAsset:
                    {
                        path = string.Format("{0}{1}", persistentDataPath, asset.path);
                    }
                    break;
            }
        }

        return path;
    }

    public static void Clear()
    {
        assets.Clear();
    }
#endregion
}

