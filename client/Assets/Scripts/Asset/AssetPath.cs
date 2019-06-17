using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public  class AssetPath
{
    #region Inner Class
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
    #endregion
    public static Dictionary<string, Asset> assets = new Dictionary<string, Asset>();
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    { 
        string path = Application.dataPath + "/asset.txt";
        string xml = File.ReadAllText(path);

        FromXml(xml);
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

