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
    }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);
        UnityEditor.EditorGUILayout.LabelField("Plugin",plugin.ToString());
        r.height += 30;
    }

    public override bool ConnectableTo(ITreeNode node)
    {
        return false;
    }

    public override Color GetConnectionColor()
    {
        return Color.yellow;
    }
#endif
}
[TreeNodeMenu("Plugin/Run")]
public partial class EntityParamPluginRun : EntityParamPlugin
{
    public EntityParamPluginRun()
    {
        plugin = typeof(ActionPluginRun);
    }
#if UNITY_EDITOR
    public override ITreeNode Clone(ITreeNode node)
    {
        return new EntityParamPluginRun();
    }
#endif
}
[TreeNodeMenu("Plugin/Remove")]
public partial class EntityParamPluginRemove : EntityParamPlugin
{
    public EntityParamPluginRemove()
    {
        plugin = typeof(ActionPluginRemove);
    }
#if UNITY_EDITOR
    public override ITreeNode Clone(ITreeNode node)
    {
        return new EntityParamPluginRemove();
    }
#endif
}
[TreeNodeMenu("Plugin/Rotate")]
public partial class EntityParamPluginRotate : EntityParamPlugin
{
    public EntityParamPluginRotate()
    {
        plugin = typeof(ActionPluginRotate);
    }
#if UNITY_EDITOR
    public override ITreeNode Clone(ITreeNode node)
    {
        return new EntityParamPluginRotate();
    }
#endif
}
[TreeNodeMenu("Plugin/Jump")]
public partial class EntityParamPluginJump : EntityParamPlugin
{
    public EntityParamPluginJump()
    {
        plugin = typeof(ActionJumpPlugin);
    }
#if UNITY_EDITOR
    public override ITreeNode Clone(ITreeNode node)
    {
        return new EntityParamPluginJump();
    }
#endif
}
public class EntityParamPluginAnimationClip
{
    public float beginAt;
    public string animationClip;
    public float length;
}
[TreeNodeMenu("Plugin/Animation")]
public abstract class EntityParamPluginAnimation : EntityParamPlugin
{
    public List<EntityParamPluginAnimationClip> animations = new List<EntityParamPluginAnimationClip>();
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
            animation.Add("beginAt",animations[i].beginAt.ToString());
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
                var animation = new EntityParamPluginAnimationClip();
                animation.beginAt = child.GetAttribute("beginAt").ToFloatEx();
                animation.animationClip = child.GetAttribute("animationClip");
                animation.length = child.GetAttribute("length").ToFloatEx();
                animations.Add(animation);
            }
        }
        base.ParseXml(node);
    }
}
[TreeNodeMenu("Plugin/Single Animation")]
public partial class EntityParamPluginSingleAnimation : EntityParamPluginAnimation
{ 
    public EntityParamPluginSingleAnimation()
    {
        plugin = typeof(ActionPluginSingleAnimation);
    }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);

        if(animations.Count > 1)
        {
            for (int i = animations.Count - 1; i >= 1; i--)
            {
                animations.RemoveAt(i);
            }
        }
       
        if (root != null)
        {
            var action = parent as EntityParamAction;

            if(action!= null && animations.Count == 0)
            {
                animations.Add(new EntityParamPluginAnimationClip());
            }

            var anims = root.GetParams<EntityParamAnimation>();
            var names = anims.Select(a => { return a.animationClip; }).ToList();
            float length = 0;
            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].beginAt = UnityEditor.EditorGUILayout.FloatField("Begin At", animations[i].beginAt);
                r.height += 18;

                if (string.IsNullOrEmpty(animations[i].animationClip))
                {
                    if (action != null)
                    {
                        animations[i].animationClip = action.action.ToString();
                    }
                }

                int index = names.IndexOf(animations[i].animationClip);

                int j = UnityEditor.EditorGUILayout.Popup("AnimationClip", index, names.ToArray());
                r.height += 18;

                EntityParamAnimation anim = null;
                if (j >= 0 && j < anims.Count )
                {
                    anim = anims[j];      
                }

                if (anim != null && j != index)
                {
                    animations[i].animationClip = anim.animationClip;
                    animations[i].length = anim.length;
                }

                UnityEditor.EditorGUILayout.BeginHorizontal();
                animations[i].length = UnityEditor.EditorGUILayout.FloatField("Length", animations[i].length);
                if (anim != null)
                {
                    animations[i].beginAt = Mathf.Clamp(animations[i].beginAt, 0, anim.length);

                    bool fix = UnityEditor.EditorGUILayout.Toggle(false);
                    if (fix)
                    {
                        animations[i].length = anim.length - animations[i].beginAt;
                    }
                }

                UnityEditor.EditorGUILayout.EndHorizontal();

                r.height += 18;

                length += animations[i].length;

            }
            if (action != null && action.duration != DEFAULT_DURATION)
            {
                action.duration = length;
            }
        }
        r.height += 10;
    }
    public override ITreeNode Clone(ITreeNode node)
    {
        var param = new EntityParamPluginSingleAnimation();

        param.animations.AddRange(animations);

        return param;
    }

#endif

   
}

[TreeNodeMenu("Plugin/Ramdon Animation")]

public partial class EntityParamPluginRamdonAnimation : EntityParamPluginAnimation
{
    public EntityParamPluginRamdonAnimation()
    {
        plugin = typeof(ActionPluginRandomAnimation);
    }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);

        int size = Mathf.Clamp(UnityEditor.EditorGUILayout.IntField("Size", animations.Count), 0, 10);
        r.height += 20;

        if (size > animations.Count)
        {
            for (int i = animations.Count; i < size; i++)
            {
                animations.Add(new EntityParamPluginAnimationClip());
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
            for (int i = 0; i < animations.Count; i++)
            {
                UnityEditor.EditorGUILayout.LabelField("  Element " + i);
                r.height += 20;

                animations[i].beginAt = UnityEditor.EditorGUILayout.FloatField("     Begin At", animations[i].beginAt);
                r.height += 20;

                if (string.IsNullOrEmpty(animations[i].animationClip))
                {
                    if (action != null)
                    {
                        animations[i].animationClip = action.action.ToString();
                    }
                }

                int index = names.IndexOf(animations[i].animationClip);

                int j = UnityEditor.EditorGUILayout.Popup("     AnimationClip", index, names.ToArray());
                r.height += 20;

                EntityParamAnimation anim = null;
                if (j >= 0 && j < anims.Count)
                {
                    anim = anims[j];
                }

                if (anim != null && j != index)
                {
                    animations[i].animationClip = anim.animationClip;
                    animations[i].length = anim.length;
                }

                UnityEditor.EditorGUILayout.BeginHorizontal();
                animations[i].length = UnityEditor.EditorGUILayout.FloatField("     Length", animations[i].length);
                if (anim != null)
                {
                    animations[i].beginAt = Mathf.Clamp(animations[i].beginAt, 0, anim.length);

                    bool fix = UnityEditor.EditorGUILayout.Toggle(false);
                    if (fix)
                    {
                        animations[i].length = anim.length - animations[i].beginAt;
                    }
                }

                UnityEditor.EditorGUILayout.EndHorizontal();

                r.height += 20;

            }
            if (action != null 
                && action.duration != DEFAULT_DURATION
                && animations.Count > 0
                )
            {
                action.duration = animations[0].length;
            }
        }
    }
    public override ITreeNode Clone(ITreeNode node)
    {
        var param = new EntityParamPluginRamdonAnimation();

        param.animations.AddRange(animations);

        return param;
    }

#endif

}
[TreeNodeMenu("Plugin/Multitude Animation")]
public partial class EntityParamPluginMultitudeAnimation : EntityParamPluginAnimation
{
    public EntityParamPluginMultitudeAnimation()
    {
        plugin = typeof(ActionPluginMultitudeAnimation);
    }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);

        int size = Mathf.Clamp(UnityEditor.EditorGUILayout.IntField("Size", animations.Count), 0, 10);
        r.height += 20;

        if (size > animations.Count)
        {
            for (int i = animations.Count; i < size; i++)
            {
                animations.Add(new EntityParamPluginAnimationClip());
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
                r.height += 20;

                animations[i].beginAt = UnityEditor.EditorGUILayout.FloatField("     Begin At", animations[i].beginAt);
                r.height += 20;

                if (string.IsNullOrEmpty(animations[i].animationClip))
                {
                    if (action != null)
                    {
                        animations[i].animationClip = action.action.ToString();
                    }
                }

                int index = names.IndexOf(animations[i].animationClip);

                int j = UnityEditor.EditorGUILayout.Popup("     AnimationClip", index, names.ToArray());
                r.height += 20;

                EntityParamAnimation anim = null;
                if (j >= 0 && j < anims.Count)
                {
                    anim = anims[j];
                }

                if (anim != null && j != index)
                {
                    animations[i].animationClip = anim.animationClip;
                    animations[i].length = anim.length;
                }

                UnityEditor.EditorGUILayout.BeginHorizontal();
                animations[i].length = UnityEditor.EditorGUILayout.FloatField("     Length", animations[i].length);
                if (anim != null)
                {
                    animations[i].beginAt = Mathf.Clamp(animations[i].beginAt, 0, anim.length);

                    bool fix = UnityEditor.EditorGUILayout.Toggle(false);
                    if (fix)
                    {
                        animations[i].length = anim.length - animations[i].beginAt;
                    }
                }

                UnityEditor.EditorGUILayout.EndHorizontal();

                r.height += 20;

                length += animations[i].length;

            }
            if (action != null
                && action.duration != DEFAULT_DURATION
                )
            {
                action.duration = length;
            }
        }
    }
    public override ITreeNode Clone(ITreeNode node)
    {
        var param = new EntityParamPluginMultitudeAnimation();

        param.animations.AddRange(animations);

        return param;
    }

#endif
}
