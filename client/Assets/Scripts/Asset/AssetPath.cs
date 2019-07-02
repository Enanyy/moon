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

        XmlAttribute name = doc.CreateAttribute("name");
        name.Value = this.name;
        node.Attributes.Append(name);
        if (string.IsNullOrEmpty(group) == false)
        {
            XmlAttribute group = doc.CreateAttribute("group");
            group.Value = this.group;
            node.Attributes.Append(group);
        }

        XmlAttribute path = doc.CreateAttribute("path");
        path.Value = this.path;
        node.Attributes.Append(path);

        XmlAttribute size = doc.CreateAttribute("size");
        size.Value = this.size.ToString();
        node.Attributes.Append(size);

        XmlAttribute md5 = doc.CreateAttribute("md5");
        md5.Value = this.md5;
        node.Attributes.Append(md5);

        XmlAttribute type = doc.CreateAttribute("type");
        type.Value = ((int)this.type).ToString();
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
    public static Dictionary<string, AssetPath> assets = new Dictionary<string, AssetPath>();

    public static AssetMode mode = AssetMode.AssetBundle;
    public const string ASSETS_FILE = "assets.txt";

    /// <summary>
    /// 结尾带/
    /// </summary>
    public static string streamingAssetsPath
    {
        get
        {
#if UNITY_ANDROID
            return string.Format("{0}/r/", Application.streamingAssetsPath);

#elif UNITY_IOS
            return string.Format("{0}/r/", Application.streamingAssetsPath);
#elif UNITY_EDITOR_OSX
            return string.Format("file://{0}/../r/{1}/", Application.dataPath,
                    UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#elif UNITY_EDITOR
            return string.Format("{0}/../r/{1}/", Application.dataPath,
                UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
            return Application.streamingAssetsPath;
#endif

        }
    }

    /// <summary>
    /// 结尾带/
    /// </summary>
    public static string persistentDataPath
    {
        get
        {
#if UNITY_ANDROID
                return string.Format("{0}/r/", Application.persistentDataPath);

#elif UNITY_IOS
                return string.Format("{0}/r/", Application.persistentDataPath);
#elif UNITY_EDITOR_OSX
                return string.Format("file://{0}/../r/{1}/", Application.dataPath,
                    UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#elif UNITY_EDITOR
            return string.Format("{0}/../r/{1}/", Application.dataPath,
                UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
            return Application.persistentDataPath;
#endif
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

            assets.Clear();

            for (int i = 0; i < root.ChildNodes.Count; ++i)
            {
                XmlElement child = root.ChildNodes[i] as XmlElement;
                AssetPath asset = new AssetPath();
                asset.FromXml(child);
                assets.Add(asset.name, asset);
                if (assets.ContainsKey(asset.path) == false)
                {
                    assets.Add(asset.path, asset);
                }
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
#if UNITY_EDITOR || UNITY_EDITOR_OSX
                    if (mode != AssetMode.AssetBundle)
                    {
                         path = Application.dataPath + "/" + asset.path;
                    }
                    else
#endif
					{
						 path = string.Format("{0}{1}", streamingAssetsPath, asset.path);
                    }
                }
                break;
                case AssetType.PersistentAsset:
                {
#if UNITY_EDITOR || UNITY_EDITOR_OSX
                    if (mode != AssetMode.AssetBundle)
                    {
                        path = Application.dataPath + "/" + asset.path;
                    }
                    else
#endif
					{
						path = string.Format("{0}{1}", persistentDataPath, asset.path);
                    }

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

