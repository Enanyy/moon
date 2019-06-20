using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
    static GUIStyle s_ToggleMixed;
    
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
            return ;
        }

        if (s_ToggleMixed == null)
        {
            s_ToggleMixed = new GUIStyle("ToggleMixed");
        }

        string name;
        string path;
        string dir;
        var asset = GetAsset(editor.target, out name, out path, out dir);

        GUILayout.BeginHorizontal();

        bool select = asset != null;
        bool mixed = editor.targets.Length > 1;
        if (mixed)
        {
            if (GUILayout.Toggle(select, "Asset", s_ToggleMixed, GUILayout.ExpandWidth(false)) == false)
            {
                RemoveAssets(editor.targets);
                asset = null;
            }
            else
            {
                AddAssets(editor.targets);
            }
        }
        else
        {
            if (GUILayout.Toggle(select, "Asset", GUILayout.ExpandWidth(false)) == false)
            {
                RemoveAssets(editor.targets);
                asset = null;
            }
            else
            {
                AddAssets(editor.targets);
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
                SaveAsset();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (asset != null)
        {
            select = string.IsNullOrEmpty(asset.group) == false;

            if (mixed)
            {
                if (GUILayout.Toggle(select, "Group",s_ToggleMixed, GUILayout.ExpandWidth(false)) == false)
                {
                    SetAssetsGroup(editor.targets,"");
                }

                else
                {
                    if (select == false)
                    {
                        asset.group = dir;
                    }

                    var group = EditorGUILayout.DelayedTextField(asset.group, GUILayout.ExpandWidth(true));
                    SetAssetsGroup(editor.targets,group);
                }
            }
            else
            {
                if (GUILayout.Toggle(select, "Group", GUILayout.ExpandWidth(false)) == false)
                {
                    asset.group = "";
                }

                else
                {
                    if (select == false)
                    {
                        asset.group = dir;
                    }

                    var group = EditorGUILayout.DelayedTextField(asset.group, GUILayout.ExpandWidth(true));
                    if (group != asset.group)
                    {
                        asset.group = group;

                        SaveAsset();
                    }
                }
            }
        }

        GUILayout.EndHorizontal();

    }

    static void AddAssets(Object[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            string name, path, dir;
            var asset = GetAsset(targets[i], out name, out path, out dir);
            if (asset == null && AssetPath.assets.ContainsKey(name) == false)
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
    }

    static void RemoveAssets(Object[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            string name, path, dir;
            var asset = GetAsset(targets[i], out name, out path, out dir);
            if (asset != null)
            {
                AssetPath.assets.Remove(asset.name);
            }
        }
    }

    static void SetAssetsGroup(Object[] targets, string group)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            string name, path, dir;
            var asset = GetAsset(targets[i], out name, out path, out dir);
            if (asset != null)
            {
                asset.group = group;
            }
        }
    }

    static AssetPath.Asset GetAsset(Object target, out string name, out string path, out string dir)
    {
        name = null;
        path = null;
        dir = null;

        string assetPath = AssetDatabase.GetAssetPath(target);

        if (string.IsNullOrEmpty(assetPath)
            || assetPath.EndsWith(".cs")
            || assetPath.StartsWith("Assets/Resources/") == false
        )
        {
            return null;
        }

        AssetPath.Asset asset = null;
        name = Path.GetFileName(assetPath);
        path = assetPath.Replace("Assets/Resources/", "").ToLower();
        dir = path.Substring(2, path.LastIndexOf('/') - 2);
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

        return asset;
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
        
    }
}

