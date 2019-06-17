using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetTool
{
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        Debug.Log("AssetPath Init");
        AssetPath.Init();
        Editor.finishedDefaultHeaderGUI -= OnPostHeaderGUI;
        Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        EditorApplication.quitting -= SaveAsset;
        EditorApplication.quitting += SaveAsset;
        EditorApplication.projectChanged -= AssetChange;
        EditorApplication.projectChanged += AssetChange;
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

        AssetPath.Asset asset = null;
        string name = Path.GetFileName(assetPath);
        string path = assetPath.Replace("Assets/Resources/", "").ToLower();
        var it = AssetPath.assets.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Value.path == path)
            {
                asset = it.Current.Value; break;
            }
        }

        if (asset == null)
        {
            string fullPath = Application.dataPath + "/Resources/" + path;
            if (File.Exists(fullPath))
            {
                string md5 = MD5Hash.Get(File.ReadAllBytes(fullPath));
                it = AssetPath.assets.GetEnumerator();
                while (it.MoveNext())
                {
                    if (it.Current.Value.md5 == md5)
                    {
                        asset = it.Current.Value;
                        if (asset.path != path)
                        {
                            asset.path = path;
                        }

                        break;
                    }
                }
            }
        }


        GUILayout.BeginHorizontal();


        if (GUILayout.Toggle(asset != null, "Asset", GUILayout.ExpandWidth(false)) == false)
        {
            if (asset != null)
            {
                AssetPath.assets.Remove(asset.name);
            }

            asset = null;
        }
        else
        {
            if (AssetPath.assets.ContainsKey(name) == false)
            {
                asset = new AssetPath.Asset();
                asset.name = name;
                asset.path = path;
                string fullPath = Application.dataPath + "/Resources/" + path;
                byte[] bytes = File.ReadAllBytes(fullPath);
                asset.md5 = MD5Hash.Get(bytes);
                asset.size = bytes.Length;

                AssetPath.assets.Add(asset.name, asset);
            }
        }

        if (asset != null)
        {
            var assetName = EditorGUILayout.DelayedTextField(asset.name, GUILayout.ExpandWidth(true));
            if (assetName != asset.name)
            {
                AssetPath.assets.Remove(asset.name);
                asset.name = assetName;
                AssetPath.assets.Add(asset.name, asset);
            }
        }
        GUILayout.EndHorizontal();
    }

    [MenuItem("Tools/生成资源配置")]
    static void GenAsset()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/Resources/r/");
        FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);

        AssetPath.Clear();

        for (int i = 0; i < files.Length; ++i)
        {
            if (files[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            string fullPath = files[i].DirectoryName + "/" + files[i].Name;
            fullPath = fullPath.Replace("\\", "/");

            string assetPath = fullPath.Substring(fullPath.IndexOf("/r/") + 1).ToLower();

            AssetPath.Asset asset = new AssetPath.Asset();

            asset.name = files[i].Name;
            asset.path = assetPath;
            asset.size = files[i].Length;

            byte[] bytes = File.ReadAllBytes(fullPath);
            asset.md5 = MD5Hash.Get(bytes);

            AssetPath.assets.Add(asset.name, asset);
        }

        SaveAsset();
    }

    static void SaveAsset()
    {
        string assetXmlFile = Application.dataPath + "/asset.txt";

        StreamWriter writerXml = new StreamWriter(assetXmlFile);
        writerXml.Write(AssetPath.ToXml());
        writerXml.Close();
        writerXml.Dispose();
    }

    static void AssetChange()
    {
        Debug.Log("Asset change");
    }
}

