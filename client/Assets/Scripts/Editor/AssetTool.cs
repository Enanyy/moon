using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public static class AssetTool
{
    [MenuItem("Tools/生成资源配置")]
    static void GenAsset()
    {
        if (EditorApplication.isCompiling)
        {
            Debug.Log("Unity Editor is compiling, please wait.");
            return;
        }

        DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/Resources/r/");
        FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);

        List<string> fileList = new List<string>();
        List<string> addList = new List<string>();
        Dictionary<uint, string> assetPathDic = AssetPath.GetAllAssets();

        Assets assets = new Assets();

        for (int i = 0; i < files.Length;++i)
        {
            if(files[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            string fullPath = files[i].DirectoryName+"/"+ files[i].Name;
            fullPath = fullPath.Replace("\\", "/");

            string assetPath = fullPath.Substring(fullPath.IndexOf("/r/") +1).ToLower();
            fileList.Add(assetPath);
            if(assetPathDic.ContainsValue(assetPath)==false)
            {
                addList.Add(assetPath);
            }
            Asset asset = new Asset();

            asset.name = files[i].Name;
            asset.path = assetPath;

            assets.assets.Add(asset.name, asset);
        }

        string assetXmlFile = Application.dataPath + "/asset.txt";

        StreamWriter writerXml = new StreamWriter(assetXmlFile);
        writerXml.Write(Assets.ToXml(assets));
        writerXml.Close();
        writerXml.Dispose();


        Dictionary<string, uint> maxIDDic = new Dictionary<string, uint>();
        List<uint> removeList = new List<uint>();
       
        var it = assetPathDic.GetEnumerator();
        while(it.MoveNext())
        {
            if(fileList.Contains(it.Current.Value)==false)
            {
                removeList.Add(it.Current.Key);
            }

            string dir = it.Current.Value.Substring(0, it.Current.Value.IndexOf('/', 2)+1);

            if (maxIDDic.ContainsKey(dir) ==false)
            {
                maxIDDic.Add(dir, it.Current.Key);
            }

            maxIDDic[dir] = maxIDDic[dir] == 0 || it.Current.Key > maxIDDic[dir] ? it.Current.Key: maxIDDic[dir];
        }

        for(int i = 0; i < removeList.Count; ++i)
        {
            assetPathDic.Remove(removeList[i]);
        }

        for(int i = 0; i < addList.Count;++i)
        {
            string dir = addList[i].Substring(0, addList[i].IndexOf('/', 2)+1);
            if (maxIDDic.ContainsKey(dir) == false)
            {
                maxIDDic.Add(dir, GetBeginAssetID(maxIDDic));
            }
            assetPathDic.Add(++maxIDDic[dir], addList[i]);
        }
        assetPathDic = assetPathDic.OrderBy(o => o.Key).ToDictionary(p => p.Key, o => o.Value);
        string assetIDString = "\n";
        string assetPathString = "\n";

        it = assetPathDic.GetEnumerator();
        while(it.MoveNext())
        {
            string path = it.Current.Value.Replace("/","_");
            path = path.Substring(0, path.LastIndexOf('.'));
            assetIDString += string.Format("\tpublic const uint {0} = {1};\n", path.ToUpper(), it.Current.Key);
            assetPathString += string.Format("\t{{ AssetID.{0},\"{1}\" }},\n", path.ToUpper(), it.Current.Value);
        }

        string assetPathFile = Application.dataPath + "/Scripts/Asset/AssetPath.cs";


        string content = File.ReadAllText(assetPathFile);

        int startIDIndex = content.IndexOf("//ASSET_ID_REPLACE_START");
        int endIDIndex = content.IndexOf("//ASSET_ID_REPLACE_END");

        string part1 = content.Substring(0, startIDIndex + "//ASSET_ID_REPLACE_START".Length + 1);
        string part2 = content.Substring(endIDIndex);

        content = part1 + assetIDString + part2;

        int startPathIndex  = content.IndexOf("//ASSET_PATH_REPLACE_START");
        int endPathIndex = content.IndexOf("//ASSET_PATH_REPLACE_END");

        part1 = content.Substring(0, startPathIndex + "//ASSET_PATH_REPLACE_START".Length + 1);
        part2 = content.Substring(endPathIndex);

        content = part1 + assetPathString + part2;

        StreamWriter writer = new StreamWriter(assetPathFile);
        writer.Write(content);
        writer.Close();
        writer.Dispose();
    }

    private static uint GetBeginAssetID(Dictionary<string, uint> maxIDDic)
    {
        uint beginid = 10000;
        int count = maxIDDic.Count;
        for(int i = 1; i<= count + 1; ++i)
        {
            bool exsit = false;
            var it = maxIDDic.GetEnumerator();
            while(it.MoveNext())
            {
                if(it.Current.Value / 10000 == i)
                {
                    exsit = true;break;
                }
            }
            if(exsit==false)
            {
                beginid = (uint)i * 10000;
                break;
            }
        }

        return beginid;
    }
}

