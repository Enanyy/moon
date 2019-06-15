using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public partial class EntityParamModel : EntityParam
{
    public string asset;
   
    public float scale = 1;
    public Vector3 hitPosition = Vector3.zero;

    private UnityEngine.Object mModel;
    private UnityEngine.GameObject mPrefab;


    public EntityParamModel() { type = EntityParamType.Model; }



#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);

     
        UnityEditor.EditorGUILayout.LabelField("Asset", asset);
        r.height += 20;
       
        scale = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Scale", scale), 0, float.MaxValue);
        r.height += 20;
        hitPosition = UnityEditor.EditorGUILayout.Vector3Field("HitPosition", hitPosition);
        r.height += 40;

      
        UnityEngine.Object obj = UnityEditor.EditorGUILayout.ObjectField("Model", mModel, typeof(UnityEngine.GameObject), false, new GUILayoutOption[0]);
        if (obj != null && obj != mModel)
        {
            asset = obj.name +".prefab";
            mModel = obj;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                var animation = children[i] as EntityParamAnimation;
                if (animation != null)
                {
                    animation.parent = null;
                    children.RemoveAt(i);
                }
            }

            CreateAnimationNode(obj);
        }
        r.height += 20;

        if(mPrefab == null && string.IsNullOrEmpty(asset)==false)
        {
            string path = string.Format("assets/resources/r/model/{0}", asset);
            mPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        var prefab = (GameObject)UnityEditor.EditorGUILayout.ObjectField("Prefab", mPrefab, typeof(UnityEngine.GameObject), false, new GUILayoutOption[0]);
        if(prefab!= null && prefab!= mPrefab)
        {
            asset = prefab.name + ".prefab";

            mPrefab = prefab;
        }
        r.height += 20;

    }

    private void CreateAnimationNode(UnityEngine.Object go)
    {
        if (go == null)
        {
            return;
        }

        string path = UnityEditor.AssetDatabase.GetAssetPath(go);

        string dirName = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1) + path.Substring(0, path.LastIndexOf('/') + 1);

        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(dirName);
        System.IO.FileInfo[] files = dir.GetFiles("*.fbx");

        int WIDTH = 240;
        int HEIGHT = 20;

        for (int i = 0; i < files.Length; ++i)
        {
            if (files[i].Name.Contains("@") == false)     // 跳过不是动作的文件
            {
                //Debug.Log("跳过无效文件：" + fileInfos[i].Name + "   "  + obj.name + "@");
                continue;
            }

            string animName = System.IO.Path.GetFileNameWithoutExtension(files[i].Name).Split('@')[1];

            string animationClip = files[i].FullName.Replace("\\", "/");
            animationClip = animationClip.Substring(animationClip.LastIndexOf("Assets/"));
            var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(animationClip);

            ActionType type = GetActionType(animName);
            EntityParamAction actionParam = null;
            for (int j = 0; j < children.Count; ++j)
            {
                var child = children[j] as EntityParamAction;
                if (child != null && type == child.action)
                {
                    actionParam = child; break;
                }
            }
            if (actionParam == null)
            {
                actionParam = new EntityParamAction();
                actionParam.rect = new Rect(300, 20, WIDTH, HEIGHT);
                AddChild(actionParam);
            }
            actionParam.action = type;
            actionParam.weight = EntityParamAction.ActionWeights[type];

            EntityParamAnimation animationParam = null;
            for (int j = 0; j < actionParam.children.Count; ++j)
            {
                var child = actionParam.children[j] as EntityParamAnimation;
                if (child != null && child.animationClip == animationClip)
                {
                    animationParam = child; break;
                }
            }
            if (animationParam == null)
            {
                animationParam = new EntityParamAnimation();
                animationParam.rect = new Rect(600, 20, WIDTH, HEIGHT);
                AddChild(animationParam);
            }
            animationParam.animationClip = animName;
            animationParam.length = clip.length;
            animationParam.mode = GetWrapMode(animName);

        }
    }

    public static ActionType GetActionType(string animName)
    {
        animName = animName.ToLower();
        if (animName.Contains("idle")
            || animName.Contains("standby")
            || animName.Contains("xiuxi")
            || animName.Contains("free"))
        {
            return ActionType.Idle;
        }
        //else if (animName.Contains("run_away")
        //   || animName.Contains("retreat"))
        //{
        //    return ActionType.Hit;
        //}
        else if (animName.Contains("run")
            || animName.Contains("walk"))
        {
            return ActionType.Run;
        }
        else if (animName.Contains("attack")
          || animName.Contains("skill")
          || animName.Contains("spell"))
        {
            return ActionType.Attack;
        }
        else if (animName.Contains("die")
          || animName.Contains("dead")
          || animName.Contains("death"))
        {
            return ActionType.Die;
        }
        else if (animName.Contains("victory"))
        {
            return ActionType.Victory;
        }
        else if (animName.Contains("hit")
            || animName.Contains("damage"))
        {
            return ActionType.Hit;
        }
        return ActionType.Idle;
    }
    public static bool IsLoop(string clipName)
    {
        if (clipName == "idle"
               || clipName == "standby"
               || clipName == "xiuxi"
               || clipName == "run_away"
               || clipName == "run"
               || clipName == "victory")
        {
            return true;
        }
        return false;
    }

    public static WrapMode GetWrapMode(string clipName)
    {
        if (IsLoop(clipName))
        {
            return WrapMode.Loop;
        }
        if (clipName.Contains("die")
          || clipName.Contains("dead")
          || clipName.Contains("death"))
        {

            return WrapMode.ClampForever;
        }

        return WrapMode.Default;
    }

    public override bool LinkAble(INode node)
    {
        if (node.GetType() == typeof(EntityParamAnimation))
        {
            return true;
        }

        if (node.GetType() == typeof(EntityParamAction))
        {
            bool linkable = true;
            var state = node as EntityParamAction;
            for (int i = 0; i < children.Count; ++i)
            {
                if (children[i].GetType() == typeof(EntityParamAction))
                {
                    var child = children[i] as EntityParamAction;
                    if (child.action == state.action)
                    {
                        linkable = false; break;
                    }
                }
            }
            return linkable;
        }
        return false;
    }

    public override INode Clone(INode node)
    {
        EntityParamModel param = node as EntityParamModel;
        if (param == null)
        {
            param = new EntityParamModel();
        }
        param.asset = this.asset;
        
        param.scale = this.scale;
        return base.Clone(param);
    }
#endif

    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }
        attributes.Add("asset", asset);
        
        attributes.Add("scale", scale.ToString());
        attributes.Add("hitPosition", hitPosition.ToString());

        return base.ToXml(parent, attributes);
    }
    public override void ParseXml(XmlElement node)
    {
        asset = node.GetAttribute("asset");
       
        scale = node.GetAttribute("scale").ToFloatEx();
        hitPosition = node.GetAttribute("hitPosition").ToVector3Ex();
        base.ParseXml(node);
    }
    public EntityParamAction GetAction(ActionType action)
    {
        for (int i = 0; i < children.Count; ++i)
        {
            var child = children[i] as EntityParamAction;
            if (child != null && child.action == action)
            {
                return child;
            }
        }
        return null;
    }

    public EntityParamAnimation GetAnimation(string name)
    {
        for(int i = 0; i < children.Count;i++)
        {
            var child = children[i] as EntityParamAnimation;
            if(child!= null && child.animationClip == name)
            {
                return child;
            }
        }
        return null;
    }
}
