using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif
/// <summary>
/// 插件参数
/// </summary>
public abstract partial class EntityParamPlugin : EntityParam
{
    public Type plugin;
    public EntityParamPlugin()
    {
        type = EntityParamType.Plugin;
        name = type.ToString();
    }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);
        UnityEditor.EditorGUILayout.LabelField(plugin.ToString());
        r.height += 20;
    }

    public override bool LinkAble(INode node)
    {
        return false;
    }

    public override Color GetColor()
    {
        return Color.yellow;
    }
#endif
}

public partial class EntityParamPluginRun : EntityParamPlugin
{
    public EntityParamPluginRun()
    {
        name = "RunPlugin";
        plugin = typeof(ActionPluginRun);
    }
#if UNITY_EDITOR
    public override INode Clone(INode node)
    {
        return new EntityParamPluginRun();
    }
#endif
}
public partial class EntityParamPluginRemove : EntityParamPlugin
{
    public EntityParamPluginRemove()
    {
        name = "RunPlugin";
        plugin = typeof(ActionPluginRemove);
    }
#if UNITY_EDITOR
    public override INode Clone(INode node)
    {
        return new EntityParamPluginRemove();
    }
#endif
}

public partial class EntityParamPluginRotate : EntityParamPlugin
{
    public EntityParamPluginRotate()
    {
        name = "RotatePlugin";
        plugin = typeof(ActionPluginRotate);
    }
#if UNITY_EDITOR
    public override INode Clone(INode node)
    {
        return new EntityParamPluginRotate();
    }
#endif
}

public partial class EntityParamPluginJump : EntityParamPlugin
{
    public EntityParamPluginJump()
    {
        name = "JumpPlugin";
        plugin = typeof(ActionJumpPlugin);
    }
#if UNITY_EDITOR
    public override INode Clone(INode node)
    {
        return new EntityParamPluginJump();
    }
#endif
}


public partial class EntityParamPluginAnimation : EntityParamPlugin
{
    public class AnimationClip
    {
        public string animationClip;
        public float length;
    }

    public List<AnimationClip> animations = new List<AnimationClip>();

    public EntityParamPluginAnimation()
    {
        name = "AnimationPlugin";
        plugin = typeof(ActionPluginAnimation);
    }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);

        int size = Mathf.Clamp(UnityEditor.EditorGUILayout.IntField("Size", animations.Count), 0, 10);

        r.height += 20;

        if (size > animations.Count)
        {
            for (int i = animations.Count; i < size; i++)
            {
                animations.Add(new AnimationClip());
            }
        }
        else if (size < animations.Count)
        {
            for (int i = animations.Count - 1; i >= size; i--)
            {
                animations.RemoveAt(i);
            }
        }

        if (root != null)
        {
            var action = parent as EntityParamAction;


            var anims = root.GetParams<EntityParamAnimation>();
            var names = anims.Select(a => { return a.animationClip; }).ToList();
            float length = 0;
            for (int i = 0; i < animations.Count; i++)
            {
                UnityEditor.EditorGUILayout.LabelField("  Element " + i);
                r.height += 18;

                if (string.IsNullOrEmpty(animations[i].animationClip))
                {
                    if (action != null)
                    {
                        animations[i].animationClip = action.action.ToString();
                    }
                }

                int index = names.IndexOf(animations[i].animationClip);

                int j = UnityEditor.EditorGUILayout.Popup("    AnimationClip", index, names.ToArray());
                r.height += 18;

                if (j >= 0 && j < anims.Count && j != index)
                {
                    animations[i].animationClip = anims[j].animationClip;
                    animations[i].length = anims[j].length;
                }

                animations[i].length = UnityEditor.EditorGUILayout.FloatField("    Length", animations[i].length);
                r.height += 18;

                length += animations[i].length;

            }
            if (action != null && action.duration != DEFAULT_DURATION)
            {
                action.duration = length;
            }

        }
    }

    public override INode Clone(INode node)
    {
        var param = new EntityParamPluginAnimation();

        param.animations.AddRange(animations);

        return param;
    }
#endif

    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }

        var node = base.ToXml(parent, attributes);

        for (int i = 0; i < animations.Count; i++)
        {
            var animation = new Dictionary<string, string>();
            animation.Add("animationClip", animations[i].animationClip);
            animation.Add("length", animations[i].length.ToString());
            CreateXmlNode(node, "AnimationClip", animation);
        }

        return node;
    }
    public override void ParseXml(XmlElement node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i] as XmlElement;
                var animation = new AnimationClip();
                animation.animationClip = child.GetAttribute("animationClip");
                animation.length = child.GetAttribute("length").ToFloatEx();
                animations.Add(animation);
            }
        }
        base.ParseXml(node);
    }
}

