using System;
using System.Xml;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;

public static class BattleParamTool
{
    public static List<TreeNodeMenu> OnInitMenu()
    {
        List<TreeNodeMenu> list = new List<TreeNodeMenu>();
        for (int i = 0; i < 4; i++)
        {
            Assembly assembly = null;
            try
            {
                switch (i)
                {
                    case 0:
                        assembly = Assembly.Load("Assembly-CSharp");
                        break;
                    case 1:
                        assembly = Assembly.Load("Assembly-CSharp-firstpass");
                        break;
                    case 2:
                        assembly = Assembly.Load("Assembly-UnityScript");
                        break;
                    case 3:
                        assembly = Assembly.Load("Assembly-UnityScript-firstpass");
                        break;
                }
            }
            catch (Exception)
            {
                assembly = null;
            }
            if (assembly != null)
            {
                Type[] types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (!types[j].IsAbstract)
                    {
                        if (types[j].IsSubclassOf(typeof(BattleParam)))
                        {
                            TreeNodeMenu menu = new TreeNodeMenu();
                            menu.name = types[j].Name.Replace("Param", "");
                            if(menu.name.Contains("Effect"))
                            {
                                menu.name = "Effect/" + menu.name;
                            }
                            else if(menu.name.Contains("Plugin"))
                            {
                                menu.name = "Plugin/" + menu.name;
                            }
                            menu.type = types[j];
                            list.Add(menu);
                        }

                    }
                }
            }
        }
        return list;
    }
    public static void OnSave(TreeNodeGraph graph)
    {
        if (graph == null)
        {
            return;
        }
        var root = graph.GetRoot();
        if (root != null)
        {
            var param = root.data as ModelParam;
            
            string path = EditorUtility.SaveFilePanel("导出特效配置文件", Application.dataPath + "/Resources/r/config/", param.model, "txt");
            path = path.ToLower();
            EditorUtility.DisplayProgressBar("请稍候", "正在导出特效配置文件", 0.1f);

            Save(param, path);

            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();

        }
    }

    public static void Save(ModelParam param, string path)
    {
        string xml = BattleParam.ToXml(param);
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, xml, System.Text.Encoding.UTF8);
        }
    }

    public static TreeNodeGraph OnLoad()
    {
        string path = EditorUtility.OpenFilePanel("Select a config", Application.dataPath + "/Resources/r/config/", "txt");

        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        Debug.Log(path);


        string text = File.ReadAllText(path);

        BattleParam param = BattleParam.Create(text);
        if(param!= null)
        {
            return CreateTreeNodeGraph(param);
        }

        return null;
    }

    public static TreeNodeGraph onDataChange()
    {
        if (Application.isPlaying)
        {
            if (BattleTest.entity != null && BattleTest.entity.param != null)
            {
                return CreateTreeNodeGraph(BattleTest.entity.param);
            }

            //foreach (var v in Selection.gameObjects)
            //{
            //    var it = BattleEffectManager.Instance.entities.GetEnumerator();
            //    while (it.MoveNext())
            //    {

            //        if (v == it.Current.Value.gameObject)
            //        {
            //            return CreateTreeNodeGraph(it.Current.Value.param);
            //        }

            //    }
            //}
        }

        return null;
    }

    [OnOpenAsset]
    private static bool OnOpenBattleParam(int instanceID,int line)
    {
        var asset = EditorUtility.InstanceIDToObject(instanceID) as TextAsset;
        if (asset != null)
        {
            BattleParam param = BattleParam.Create(asset.text);
            if (param != null)
            {
                TreeNodeGraph graph = CreateTreeNodeGraph(param);
                if (graph != null)
                {
                    TreeNodeWindow.SetData(graph);
                    return true;
                }
            }
        }
        return false;
    }
   
    

    public static TreeNodeGraph CreateTreeNodeGraph(BattleParam root)
    {
        if(root == null)
        {
            return null;
        }
        TreeNodeGraph graph = new TreeNodeGraph();

       
        var node = graph.AddNode(root);
        AddChildNode(ref graph, ref node, root);
        return graph;
    }

    private static void AddChildNode(ref TreeNodeGraph graph, ref TreeNode parent,BattleParam param)
    {
        if(parent != null)
        {
            for(int i = 0; i < param.children.Count; ++i)
            {
                TreeNode node = graph.AddNode(param.children[i]);
                node.parent = parent;
                AddChildNode(ref graph, ref node, param.children[i]);
            }
        }
    }
    

}
