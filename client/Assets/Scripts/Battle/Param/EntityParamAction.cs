using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

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



    public EntityParamAction() { base.type = EntityParamType.Action; name = base.type.ToString(); }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);

        action = (ActionType)UnityEditor.EditorGUILayout.EnumPopup("Action", action);
        r.height += 20;
        weight = UnityEditor.EditorGUILayout.IntField("Weight", weight);
        r.height += 20;
        UnityEditor.EditorGUILayout.BeginHorizontal();
        duration = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Duration", duration), 0, float.MaxValue);
        bool loop = duration == DEFAULT_DURATION;
        loop = UnityEditor.EditorGUILayout.Toggle("", loop);
        if (loop)
        {
            duration = DEFAULT_DURATION;
        }
        UnityEditor.EditorGUILayout.EndHorizontal();
        r.height += 20;
    }
    public override bool LinkAble(INode node)
    {
        return node.GetType().IsSubclassOf(typeof(EntityParamPlugin));
    }

    public override Color GetColor()
    {
        return Color.blue;
    }

    public override INode Clone(INode node)
    {
        EntityParamAction param = node as EntityParamAction;
        if (param == null)
        {
            param = new EntityParamAction();
        }
        param.action = this.action;
        param.weight = this.weight;
        param.duration = this.duration;

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

        return base.ToXml(parent, attributes);

    }

    public override void ParseXml(XmlElement node)
    {
        action = node.GetAttribute("action").ToEnumEx<ActionType>();
        weight = node.GetAttribute("weight").ToInt32Ex();
        duration = node.GetAttribute("duration").ToFloatEx();

        base.ParseXml(node);
    }
}
