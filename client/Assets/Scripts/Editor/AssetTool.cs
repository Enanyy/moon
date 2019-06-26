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
  
    
    static  Dictionary<string,AssetType> s_AssetDirs = new Dictionary<string, AssetType>
    {
        { "Assets/Resources/",AssetType.Resource },
        { "Assets/R/",AssetType.StreamingAsset },
    };

    static bool IsValid(string assetPath)
    {
        bool isValid = false;

        if (string.IsNullOrEmpty(assetPath)
            || assetPath.EndsWith(".cs")
        )
        {
            return isValid;
        }
        var it = s_AssetDirs.GetEnumerator();
        while (it.MoveNext())
        {
            if (assetPath.StartsWith(it.Current.Key))
            {
                isValid = true; break;
            }
        }

        return isValid;
    }
    static void OnPostHeaderGUI(Editor editor)
    {
        if (editor.target == null)
        {
            return;
        }
        string assetPath = AssetDatabase.GetAssetPath(editor.target);
        
        if (IsValid(assetPath)==false)
        {
            return;
        }

        if (s_ToggleMixed == null)
        {
            s_ToggleMixed = new GUIStyle("ToggleMixed");
        }

        var asset = GetAsset(editor.target);

        GUILayout.BeginHorizontal();

        bool select = asset != null && AssetPath.assets.ContainsKey(asset.name);
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
                        string dir = Path.GetDirectoryName(asset.path);
                        dir = dir.Substring(dir.LastIndexOf("\\") + 1);

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
                        string dir = Path.GetDirectoryName(asset.path);
                        dir = dir.Substring(dir.LastIndexOf("\\") + 1);

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
            var asset = GetAsset(targets[i]);
            if (asset != null && AssetPath.assets.ContainsKey(asset.name) == false)
            {
                AssetPath.assets.Add(asset.name, asset);
            }
        }
    }

    static void RemoveAssets(Object[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            var asset = GetAsset(targets[i]);
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
            var asset = GetAsset(targets[i]);
            if (asset != null)
            {
                asset.group = group;
            }
        }
    }

    static AssetType GetAssetType(string assetPath)
    {
        var it = s_AssetDirs.GetEnumerator();
        while (it.MoveNext())
        {
            if (assetPath.StartsWith(it.Current.Key))
            {
                return it.Current.Value;
            }
        }

        return AssetType.Resource;
    }

    static string GetAssetPath(string assetPath)
    {
        var type = GetAssetType(assetPath);
        string path = assetPath.ToLower();
        if (type == AssetType.Resource)
        {
            path = assetPath.ToLower().Replace("assets/resources/", "");
        }

        return path;
    }

    static Asset GetAsset(Object target)
    {
        string assetPath = AssetDatabase.GetAssetPath(target);

        if (IsValid(assetPath) == false)
        {
            return null;
        }

        string fullpath = Application.dataPath.Replace("Assets", "/") + assetPath;
        byte[] bytes = File.Exists(fullpath) ? File.ReadAllBytes(fullpath) : null;

        string name = Path.GetFileName(assetPath);
        string path = GetAssetPath(assetPath);
        string md5 = bytes != null ? MD5Hash.Get(bytes) : "";
        AssetType type = GetAssetType(assetPath);

        Asset asset = AssetPath.Get(name);
        if (asset == null)
        {
            var it = AssetPath.assets.GetEnumerator();
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

        if (asset == null && bytes != null)
        {
            asset = new Asset();
            asset.name = name;
            asset.path = path;
            asset.md5 = md5;
            asset.type = type;
            asset.size = bytes.Length;
        }


        return asset;
    }

    [MenuItem("Tools/生成资源配置")]
    static void GenAsset()
    {
        AssetPath.Clear();
        var it = s_AssetDirs.GetEnumerator();
        while (it.MoveNext())
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + it.Current.Key.Replace("Assets",""));
            if (dirInfo.Exists == false)
            {
                continue;
            }
            FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            AssetType type = it.Current.Value;
           

            for (int j = 0; j < files.Length; ++j)
            {
                if (files[j].Name.EndsWith(".meta"))
                {
                    continue;
                }
                string fullPath = files[j].DirectoryName + "/" + files[j].Name;
                fullPath = fullPath.Replace("\\", "/");


                Asset asset = new Asset();

                asset.name = files[j].Name;

                asset.size = files[j].Length;

                asset.md5 = MD5Hash.Get(File.ReadAllBytes(fullPath));

                asset.type = type;
                if (type == AssetType.Resource)
                {
                    asset.path = fullPath.Substring(fullPath.IndexOf(it.Current.Key ) + it.Current.Key.Length).ToLower(); ;
                }
                else
                {
                    asset.path = fullPath.Substring(fullPath.IndexOf(it.Current.Key)).ToLower();
                }

                AssetPath.assets.Add(asset.name, asset);
            }
        }
        

        SaveAsset();
    }
    [MenuItem("Tools/保存资源配置")]
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

