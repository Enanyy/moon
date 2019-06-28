using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[TreeNodeMenu("Animation")]
public partial class EntityParamAnimation : EntityParam
{
    public string animationClip;
    public float length;
    public WrapMode mode;

    public EntityParamAnimation() { type = EntityParamType.Animation; }
#if UNITY_EDITOR
    private AnimationClip mAnimationClip;
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);

        UnityEditor.EditorGUILayout.LabelField("AnimationClip", animationClip);
        r.height += 20;

        UnityEditor.EditorGUILayout.LabelField("Length", length.ToString());
        r.height += 20;

        mode = (WrapMode)UnityEditor.EditorGUILayout.EnumPopup("Mode", mode);
        r.height += 20;

        if(mAnimationClip == null && string.IsNullOrEmpty(animationClip)==false && parent != null)
        {
            EntityParamModel model = parent as EntityParamModel;
            if(model!= null)
            {
                mAnimationClip = model.GetAnimationClip(animationClip);
            }
        }


        AnimationClip clip = (AnimationClip)UnityEditor.EditorGUILayout.ObjectField(mAnimationClip, typeof(AnimationClip), false, new GUILayoutOption[0]);
        if (clip != null && mAnimationClip != clip)
        {
            mAnimationClip = clip;
            animationClip = mAnimationClip.name;
            length = mAnimationClip.length;
            mode = mAnimationClip.wrapMode;
        }
        r.height += 20;
    }
    public override bool ConnectableTo(ITreeNode node)
    {
        return node.GetType().IsSubclassOf(typeof(EntityParamEffect));
    }

    public override ITreeNode Clone(ITreeNode node)
    {
        EntityParamAnimation param = node as EntityParamAnimation;
        if (param == null)
        {
            param = new EntityParamAnimation();
        }
        param.animationClip = this.animationClip;
        param.length = this.length;
        param.mode = this.mode;

        return base.Clone(param);
    }

#endif
    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }
        attributes.Add("animationClip", animationClip);
        attributes.Add("length", length.ToString());
        attributes.Add("mode", mode.ToString());


        return base.ToXml(parent, attributes);
    }

    public override void ParseXml(XmlElement node)
    {
        animationClip = node.GetAttribute("animationClip");
        length = node.GetAttribute("length").ToFloatEx();
        mode = node.GetAttribute("mode").ToEnumEx<WrapMode>();


        base.ParseXml(node);
    }
}