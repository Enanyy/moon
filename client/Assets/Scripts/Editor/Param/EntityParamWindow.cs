using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class EntityParamWindow : TreeNodeWindow
{
    [MenuItem("Tools/Param Editor")]
    private static void ShowEditor()
    {
        Open<EntityParamWindow>(CreateTemplate());
    }

    protected override TreeNodeGraph OnNew()
    {
        return CreateTemplate();
    }

    private static TreeNodeGraph CreateTemplate()
    {
        var modelParam = new EntityParamModel();
        modelParam.rect = new Rect(20, 20, TreeNode.WIDTH, TreeNode.HEIGHT);

        var actionParam = new EntityParamAction();
        actionParam.rect = new Rect(300, 20, TreeNode.WIDTH, TreeNode.HEIGHT);
        modelParam.AddChild(actionParam);

        var animationParam = new EntityParamAnimation();
        animationParam.rect = new Rect(600, 20, TreeNode.WIDTH, TreeNode.HEIGHT);
        modelParam.AddChild(animationParam);

        return TreeNodeGraph.CreateGraph(modelParam);
    }

    protected override TreeNodeGraph OnLoad()
    {
        string path = EditorUtility.OpenFilePanel("Select a config", Application.dataPath + "/Resources/r/config/", "txt");

        if (string.IsNullOrEmpty(path))
        {
            return null ;
        }

        Debug.Log(path);


        string text = File.ReadAllText(path);

        EntityParam param = EntityParam.Create(text);
        if (param != null)
        {
            return TreeNodeGraph.CreateGraph(param);
        }

        return base.OnLoad();
    }

    protected override void OnSave(TreeNodeGraph graph)
    {
        if (graph == null)
        {
            return;
        }
        var root = graph.GetRoot();
        if (root != null)
        {
            var param = root.data as EntityParamModel;

            string name = "model";
            if (string.IsNullOrEmpty(param.asset) == false)
            {
                name = param.asset.Replace(".prefab", "");
            }
            string path = EditorUtility.SaveFilePanel("导出特效配置文件", Application.dataPath + "/Resources/r/config/", name, "txt");
            path = path.ToLower();
            EditorUtility.DisplayProgressBar("请稍候", "正在导出特效配置文件", 0.1f);

            Save(param, path);

            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();

        }
    }
    public static void Save(EntityParamModel param, string path)
    {
        string xml = EntityParam.ToXml(param);
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, xml, System.Text.Encoding.UTF8);
        }
    }

    [OnOpenAsset]
    private static bool OnOpenBattleParam(int instanceID, int line)
    {
        var asset = EditorUtility.InstanceIDToObject(instanceID) as TextAsset;
        if (asset != null)
        {
            EntityParam param = EntityParam.Create(asset.text);
            if (param != null)
            {
                TreeNodeGraph graph = TreeNodeGraph.CreateGraph(param);
                if (graph != null)
                {
                    Open<EntityParamWindow>(graph);
                    return true;
                }
            }
        }
        return false;
    }
}

