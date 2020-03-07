using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[TreeNodeMenu("Action")]
public partial class EntityParamAction : EntityParam
{
    public static readonly Dictionary<ActionType, int> ActionWeights = new Dictionary<ActionType, int>
    {
        {ActionType.Idle,0 },
        {ActionType.Run,1 },
        {ActionType.Attack,1 },
        {ActionType.Die,10 },
        {ActionType.Hit,10 },
        {ActionType.Victory,10 },
    };

    public ActionType action;
    public int weight;
    public float duration;
    public float beforeAt;
    public float afterAt;


    public EntityParamAction() { base.type = EntityParamType.Action; }
#if UNITY_EDITOR
    public override void OnDraw(ref Rect r)
    {
        base.OnDraw(ref r);

        action = (ActionType)UnityEditor.EditorGUILayout.EnumPopup("Action", action);
        r.height += 20;
        weight = UnityEditor.EditorGUILayout.IntField("Weight", weight);
        r.height += 20;
        UnityEditor.EditorGUILayout.BeginHorizontal();
        duration = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Duration", duration), 0, float.MaxValue);
        bool loop = duration == DEFAULT_DURATION;
        loop = UnityEditor.EditorGUILayout.Toggle("", loop);
        UnityEditor.EditorGUILayout.EndHorizontal();
        if (loop)
        {
            duration = DEFAULT_DURATION;
            beforeAt = -1;
            afterAt = -1;
        }
        else
        {
            r.height += 20;
            beforeAt = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("BeforeAt", beforeAt), 0, duration);
            r.height += 20;
            afterAt = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("AfterAt", afterAt), 0, duration);
        }
        
        r.height += 30;
    }
    public override bool ConnectableTo(ITreeNode node)
    {
        Type type = node.GetType();
        if (type.IsSubclassOf(typeof(EntityParamPluginAnimation)))
        {
            if (GetParams<EntityParamPluginAnimation>().Count > 0)
            {
                return false;
            }
        }
        return type.IsSubclassOf(typeof(EntityParamPlugin));
    }

    public override Color GetConnectionColor()
    {
        return Color.blue;
    }

    public override ITreeNode Clone(ITreeNode node)
    {
        EntityParamAction param = node as EntityParamAction;
        if (param == null)
        {
            param = new EntityParamAction();
        }
        param.action = this.action;
        param.weight = this.weight;
        param.duration = this.duration;
        param.beforeAt = this.beforeAt;
        param.afterAt = this.afterAt;

        return base.Clone(param);
    }
#endif
    public override XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }
        attributes.Add("action", action.ToString());
        attributes.Add("weight", weight.ToString());
        attributes.Add("duration", duration.ToString());
        attributes.Add("beforeAt", beforeAt.ToString());
        attributes.Add("afterAt", afterAt.ToString());

        return base.ToXml(parent, attributes);

    }

    public override void ParseXml(XmlElement node)
    {
        action = node.GetAttribute("action").ToEnumEx<ActionType>();
        weight = node.GetAttribute("weight").ToInt32Ex();
        duration = node.GetAttribute("duration").ToFloatEx();
        beforeAt = node.GetAttribute("beforeAt").ToFloatEx();
        afterAt = node.GetAttribute("afterAt").ToFloatEx();

        base.ParseXml(node);
    }
}
