using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public abstract partial class EntityParamEffect : EntityParam
{
    public EffectType effectType { get; protected set; }
    public EffectArise arise;
    public EffectOn on;


    public string asset;     //资源ID

    public float delay;      //延迟

    public Vector3 offset;

    public EntityParamEffect() { type = EntityParamType.Effect;  }
#if UNITY_EDITOR

    private GameObject mPrefab;

    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);
        UnityEditor.EditorGUILayout.LabelField("EffectType", effectType.ToString());
        r.height += 20;

        if (parent != null)
        {
            if (parent.GetType() == typeof(EntityParamAnimation))
            {
                arise = EffectArise.ParentBegin;
            }
            else if(parent.GetType() == typeof(EntityParamEffectTime))
            {
                arise = (EffectArise)UnityEditor.EditorGUILayout.EnumPopup("Arise", arise);
                r.height += 20;
                if (arise == EffectArise.ParentTrigger)
                {
                    arise = EffectArise.ParentBegin;
                }
            }
            else
            {
                arise = (EffectArise)UnityEditor.EditorGUILayout.EnumPopup("Arise", arise);
                r.height += 20;
            }        
        }
        else
        {
            arise = (EffectArise)UnityEditor.EditorGUILayout.EnumPopup("Arise", arise);
            r.height += 20;
        }
        on = (EffectOn)UnityEditor.EditorGUILayout.EnumPopup("On", on);
        r.height += 20;
        asset = UnityEditor.EditorGUILayout.TextField("Asset", asset);
        r.height += 20;

        if (mPrefab == null && string.IsNullOrEmpty(asset) == false)
        {
            string path = string.Format("assets/resources/r/spell_fx/{0}", asset);
            mPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (mPrefab == null)
            {
                path = string.Format("assets/r/spell_fx/{0}", asset);
                mPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
        }

        var obj = (GameObject)UnityEditor.EditorGUILayout.ObjectField("Prefab", mPrefab, typeof(UnityEngine.GameObject), false, new GUILayoutOption[0]);
        if (obj != null && obj != mPrefab)
        {
            asset = obj.name + ".prefab";
            mPrefab = obj;
        }
        r.height += 20;
        delay = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Delay", delay), 0, float.MaxValue);
        r.height += 20;

        offset = UnityEditor.EditorGUILayout.Vector3Field("Offset", offset);
        r.height += 25;


    }
    public override bool ConnectableTo(ITreeNode node)
    {
        return node.GetType().IsSubclassOf(typeof(EntityParamEffect)) && node != this;
    }
    public override ITreeNode Clone(ITreeNode node)
    {
        EntityParamEffect param = node as EntityParamEffect;
        if (param != null)
        {
            param.effectType = this.effectType;
            param.arise = this.arise;
            param.on = this.on;


            param.asset = this.asset;
            param.delay = this.delay;
            param.offset = this.offset;

        }


        return base.Clone(param);
    }
#endif

    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }
        attributes.Add("effectType", effectType.ToString());
        attributes.Add("arise", arise.ToString());
        attributes.Add("on", on.ToString());

        attributes.Add("asset", asset);
        attributes.Add("delay", delay.ToString());
        attributes.Add("offset", offset.ToString());

        return base.ToXml(parent, attributes);
    }

    public override void ParseXml(XmlElement node)
    {
        effectType = node.GetAttribute("effectType").ToEnumEx<EffectType>();
        arise = node.GetAttribute("arise").ToEnumEx<EffectArise>();
        on = node.GetAttribute("on").ToEnumEx<EffectOn>();
        asset = node.GetAttribute("asset");
        delay = node.GetAttribute("delay").ToFloatEx();
        offset = node.GetAttribute("offset").ToVector3Ex();
        base.ParseXml(node);
    }

}

public partial class EntityParamEffectTime : EntityParamEffect
{
    public BonePoint bone;
    public bool bind = false;//绑定
    public bool syncAnimationSpeed = false; //特效速度是否同步动作速度
    public float duration;

    public EntityParamEffectTime()
    {
        effectType = EffectType.Time;
    }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);
        duration = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Duration", duration), 0, float.MaxValue);
        r.height += 20;

        bone = (BonePoint)UnityEditor.EditorGUILayout.EnumPopup("Bone", bone);
        r.height += 20;
        if (bone != BonePoint.None)
        {
            bind = UnityEditor.EditorGUILayout.Toggle("Bind", bind);
            r.height += 20;
        }
        syncAnimationSpeed = UnityEditor.EditorGUILayout.Toggle("SyncAnimationSpeed", syncAnimationSpeed);
        r.height += 20;

    }

    public override ITreeNode Clone(ITreeNode node)
    {
        EntityParamEffectTime param = node as EntityParamEffectTime;
        if (param == null)
        {
            param = new EntityParamEffectTime();
        }
        param.duration = this.duration;

        param.bind = this.bind;
        param.bone = this.bone;
        param.syncAnimationSpeed = this.syncAnimationSpeed;
        return base.Clone(param);
    }
#endif
    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }
        attributes.Add("duration", duration.ToString());

        attributes.Add("bone", bone.ToString());
        attributes.Add("bind", (bind ? 1 : 0).ToString());
        attributes.Add("syncAnimationSpeed", (syncAnimationSpeed ? 1 : 0).ToString());

        return base.ToXml(parent, attributes);
    }

    public override void ParseXml(XmlElement node)
    {
        duration = node.GetAttribute("duration").ToFloatEx();
        bone = node.GetAttribute("bone").ToEnumEx<BonePoint>();
        bind = node.GetAttribute("bind").ToInt32Ex() == 1;
        syncAnimationSpeed = node.GetAttribute("syncAnimationSpeed").ToInt32Ex() == 1;

        base.ParseXml(node);
    }
}

public partial class EntityParamEffectMove : EntityParamEffect
{
    public float speed;
    public float distance;

    public Vector3 direction;

    public EntityParamEffectMove()
    {
        effectType = EffectType.Move;
    }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);
        distance = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Distance", distance), 0, float.MaxValue);
        r.height += 20;
        speed = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Speed", speed), 0, float.MaxValue);
        r.height += 20;
        direction = UnityEditor.EditorGUILayout.Vector3Field("Direction", direction);
        r.height += 25;
    }
    public override ITreeNode Clone(ITreeNode node)
    {
        EntityParamEffectMove param = node as EntityParamEffectMove;
        if (param == null)
        {
            param = new EntityParamEffectMove();
        }
        param.distance = this.distance;
        param.speed = this.speed;
        param.direction = this.direction;

        return base.Clone(param);
    }
#endif
    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }
        attributes.Add("distance", distance.ToString());
        attributes.Add("speed", speed.ToString());
        attributes.Add("direction", direction.ToString());
        return base.ToXml(parent, attributes);
    }

    public override void ParseXml(XmlElement node)
    {
        distance = node.GetAttribute("distance").ToFloatEx();
        speed = node.GetAttribute("speed").ToFloatEx();
        direction = node.GetAttribute("direction").ToVector3Ex();
        base.ParseXml(node);
    }
}

public partial class EntityParamEffectFollow : EntityParamEffect
{
    public float speed;

    public EntityParamEffectFollow()
    {
        effectType = EffectType.Follow;
    }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);
        speed = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Speed", speed), 0, float.MaxValue);
        r.height += 20;

    }
    public override ITreeNode Clone(ITreeNode node)
    {
        EntityParamEffectFollow param = node as EntityParamEffectFollow;
        if (param == null)
        {
            param = new EntityParamEffectFollow();
        }
        param.speed = this.speed;

        return base.Clone(param);
    }
#endif
    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }
        attributes.Add("speed", speed.ToString());

        return base.ToXml(parent, attributes);
    }

    public override void ParseXml(XmlElement node)
    {
        speed = node.GetAttribute("speed").ToFloatEx();

        base.ParseXml(node);
    }
}

public partial class EntityParamEffectParabola : EntityParamEffect
{
    public float speed;      //速度
    public float gravity;    //重力（抛物线特效使用）
    public float heightOffset;
    public float heightLimit;
    public EntityParamEffectParabola()
    {
        effectType = EffectType.Parabola;
    }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);
        speed = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Speed", speed), 0, float.MaxValue);
        r.height += 20;
        gravity = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Gravity", gravity), 0, 100);
        r.height += 20;

        heightOffset = UnityEditor.EditorGUILayout.FloatField("Height Offset", heightOffset);
        r.height += 20;
        heightLimit = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Height Limit", heightLimit), 0, 100);
        r.height += 20;
    }
    public override ITreeNode Clone(ITreeNode node)
    {
        EntityParamEffectParabola param = node as EntityParamEffectParabola;
        if (param == null)
        {
            param = new EntityParamEffectParabola();
        }
        param.speed = this.speed;
        param.gravity = this.gravity;
        param.heightOffset = this.heightOffset;
        param.heightLimit = this.heightLimit;

        return base.Clone(node);
    }
#endif
    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }
        attributes.Add("speed", speed.ToString());
        attributes.Add("gravity", gravity.ToString());
        attributes.Add("heightOffset", heightOffset.ToString());
        attributes.Add("heightLimit", heightLimit.ToString());
        return base.ToXml(parent, attributes);
    }

    public override void ParseXml(XmlElement node)
    {
        speed = node.GetAttribute("speed").ToFloatEx();
        gravity = node.GetAttribute("gravity").ToFloatEx();
        heightOffset = node.GetAttribute("heightOffset").ToFloatEx();
        heightLimit = node.GetAttribute("heightLimit").ToFloatEx();
        base.ParseXml(node);
    }
}
