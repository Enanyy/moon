﻿using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public partial class EntityParamAnimation : EntityParam
{
    public string animationClip;
    public float length;
    public WrapMode mode;

    private AnimationClip mAnimationClip;

    public EntityParamAnimation() { type = EntityParamType.Animation; }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);

        UnityEditor.EditorGUILayout.LabelField("AnimationClip", animationClip);
        r.height += 20;

        UnityEditor.EditorGUILayout.LabelField("Length", length.ToString());
        r.height += 20;

        mode = (WrapMode)UnityEditor.EditorGUILayout.EnumPopup("Mode", mode);
        r.height += 20;

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
    public override bool LinkAble(INode node)
    {
        return node.GetType().IsSubclassOf(typeof(EntityParamEffect));
    }

    public override INode Clone(INode node)
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