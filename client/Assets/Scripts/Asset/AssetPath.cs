using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Asset
{
    public string name;
    public string path;
    public long size;
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

        XmlAttribute name = doc.CreateAttribute("name");
        name.Value = this.name;

        node.Attributes.Append(name);

        XmlAttribute path = doc.CreateAttribute("path");
        path.Value = this.path;

        node.Attributes.Append(path);

        XmlAttribute size = doc.CreateAttribute("size");
        size.Value = this.size.ToString();

        node.Attributes.Append(size);

        XmlAttribute md5 = doc.CreateAttribute("md5");
        md5.Value = this.md5;

        node.Attributes.Append(md5);
    }

    public void FromXml(XmlElement element)
    {
        name = element.GetAttribute("name");
        path = element.GetAttribute("path");
        size = element.GetAttribute("size").ToInt64Ex();
        md5 = element.GetAttribute("md5");
    }
}

public  class AssetPath
{
    public static Dictionary<string, Asset> assets = new Dictionary<string, Asset>();
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    { 
        string path = Application.dataPath + "/asset.txt";
        string xml = File.ReadAllText(path);

        FromXml(xml);
    }
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        Debug.Log("AssetPath Init");
        Init();
        Editor.finishedDefaultHeaderGUI -= OnPostHeaderGUI;
        Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
    }

    static void OnPostHeaderGUI(Editor editor)
    {
        if (editor.target == null)
        {
            return;
        }
        string assetPath = AssetDatabase.GetAssetPath(editor.target);

        if (string.IsNullOrEmpty(assetPath) 
            || assetPath.EndsWith(".cs")
            || assetPath.StartsWith("Assets/Resources/") == false
            )
        {
            return;
        }

        Asset asset = null;
        string name = Path.GetFileNameWithoutExtension(assetPath);
        string path = assetPath.Replace("Assets/Resources/", "").ToLower();
        var it = assets.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Value.path == path)
            {
                asset = it.Current.Value;break;
            }
        }


        GUILayout.BeginHorizontal();


        if (GUILayout.Toggle(asset!= null, "Asset", GUILayout.ExpandWidth(false)) == false)
        {
            assets.Remove(asset.name);

            asset = null;
        }

        if (asset != null)
        {
            var assetName = EditorGUILayout.DelayedTextField(asset.name, GUILayout.ExpandWidth(true));
            if (assetName != asset.name)
            {
                asset.name = assetName;
            }
        }
        GUILayout.EndHorizontal();
    }

#endif
    public static void FromXml(string xml)
    {
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(xml);

        XmlElement root = doc.DocumentElement;

        assets.Clear();

        for (int i = 0; i < root.ChildNodes.Count; ++i)
        {
            XmlElement child = root.ChildNodes[i] as XmlElement;
            Asset asset = new Asset();
            asset.FromXml(child);
            assets.Add(asset.name, asset);
        }
    }

    public static string ToXml()
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        doc.InsertBefore(dec, doc.DocumentElement);

        XmlElement root = doc.CreateElement("Assets");
        doc.AppendChild(root);
        var it = assets.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.ToXml(root);
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
    public static string Get(string name)
    {
        Asset asset;
        assets.TryGetValue(name, out asset);
        if(asset!= null)
        {
            return asset.path;
        }
        return "";
    }
    public static void Clear()
    {
        assets.Clear();
    }
}

