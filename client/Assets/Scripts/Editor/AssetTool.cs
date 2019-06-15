using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetTool
{
    [MenuItem("Tools/生成资源配置")]
    static void GenAsset()
    {
   
        DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/Resources/r/");
        FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);

        AssetPath.Clear();
       
        for (int i = 0; i < files.Length;++i)
        {
            if(files[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            string fullPath = files[i].DirectoryName+"/"+ files[i].Name;
            fullPath = fullPath.Replace("\\", "/");

            string assetPath = fullPath.Substring(fullPath.IndexOf("/r/") +1).ToLower();
           
            Asset asset = new Asset();

            asset.name = files[i].Name;
            asset.path = assetPath;
            asset.size = files[i].Length;

            byte[] bytes = File.ReadAllBytes(fullPath);
            asset.md5 = MD5Hash.Get(bytes);

            AssetPath.assets.Add(asset.name, asset);
        }

        string assetXmlFile = Application.dataPath + "/asset.txt";

        StreamWriter writerXml = new StreamWriter(assetXmlFile);
        writerXml.Write(AssetPath.ToXml());
        writerXml.Close();
        writerXml.Dispose();
    }
}

