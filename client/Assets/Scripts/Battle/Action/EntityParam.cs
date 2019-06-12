using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

#if UNITY_EDITOR
using System.Linq;
public interface INode
{
    string name { get; set; }
    Rect rect { get; set; }
    void Draw(ref Rect rect);
    bool LinkAble(INode node);
    void OnLink(INode node);
    void OnUnLink(INode node);
    INode Clone(INode node);
    Color GetColor();
}
#endif


public enum EffectArise
{
    ParentBegin,
    ParentTrigger,
    ParentEnd,
}

public enum EffectOn
{
    Self,//
    Target,
    Parent
}

public enum EffectType
{
    None,
    Time,
    Move,
    Follow,
    Parabola,
}

public enum BoneType
{
    None,
    Root,      // Root_node
    Buff,      // Bone_Buff     
    Head,      // Bone_Head 
    Hit,       // Bone_Hit    
    Weapon1,   // Bip001 Prop1 （武器点1）  
    Weapon2,   // Bip001 Prop2 （武器点2）
    Weapon3,   // Bip001 Prop3 （武器点3）
}

public enum EntityParamType
{
    Model,
    Action,
    Animation,
    Effect,
    Plugin,
}

public enum ActionType
{
    Idle,
    Run,
    Attack,
    Die,
    Victory,
    Hit,
    Jump,
}
public abstract partial class EntityParam
#if UNITY_EDITOR    
    :INode
#endif
{
    public EntityParamType type { get; protected set; }
    public string name { get; set; }
    public Rect rect { get; set; }
    public List<EntityParam> children = new List<EntityParam>();

    public EntityParam parent { get; set; }

    public const float DEFAULT_DURATION = 86400;
    public EntityParam root
    {
        get
        {
            var param = this;
            while (param.parent!= null)
            {
                param = param.parent;
            }

            return param;
        }
    }

    public EntityParam()
    {
        name = type.ToString();
    }

   
    public virtual XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }

        attributes.Add("name", name);
        attributes.Add("rect", rect.ToStringEx());

        var node = CreateXmlNode(parent, GetType().ToString(), attributes);

        for (int i = 0; i < children.Count; ++i)
        {
            children[i].ToXml(node);
        }

        return node;
    }


    public static XmlElement CreateXmlNode(XmlNode parent, string tag, Dictionary<string, string> attributes)
    {
        XmlDocument doc;
        if (parent.ParentNode == null)
        {
            doc = (XmlDocument)parent;
        }
        else
        {
            doc = parent.OwnerDocument;
        }
        XmlElement node = doc.CreateElement(tag);

        parent.AppendChild(node);

        foreach (var v in attributes)
        {
            //创建一个属性
            XmlAttribute attribute = doc.CreateAttribute(v.Key);
            attribute.Value = v.Value;
            //xml节点附件属性
            node.Attributes.Append(attribute);
        }

        return node;
    }


    public virtual void ParseXml(XmlElement node)
    {
        if(node!= null)
        {
            name = node.GetAttribute("name");
            rect = node.GetAttribute("rect").ToRectEx();
            if(node.ChildNodes != null)
            {
                for(int i = 0; i< node.ChildNodes.Count; ++ i)
                {
                    var child = node.ChildNodes[i] as XmlElement;
                    Type type = Type.GetType(child.Name);
                    if (type != null && type.IsSubclassOf(typeof(EntityParam)))
                    {
                        var param = (EntityParam)Activator.CreateInstance(type);
                        if (param != null)
                        {
                            param.ParseXml(child);
                            param.parent = this;
                            children.Add(param);
                        }
                    }
                }
            }
        }
    }

    public List<T> GetParams<T>() where T : EntityParam
    {
        List<T> list = new List<T>();
        for (int i = 0; i < children.Count; i++)
        {
            var param = children[i] as T;
            if (param != null)
            {
                list.Add(param);
            }
        }

        return list;
    }

    public static bool IsTypeOrSubClass(string tag, Type subClass)
    {
        if (string.IsNullOrEmpty(tag))
        {
            return false;
        }
        Type type = Type.GetType(tag);
        if (type != null)
        {
            return type == subClass || type.IsSubclassOf(subClass);
        }
        return false;
    }

    public static EntityParam Create(string xml)
    {
        try
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(xml);

            for (int i = 0; i < doc.ChildNodes.Count; ++i)
            {
                var child = doc.ChildNodes[i] as XmlElement;
                if (child != null)
                {
                    EntityParam param = (EntityParam)Activator.CreateInstance(Type.GetType(child.Name));
                    if (param != null)
                    {
                        param.ParseXml(child);

                        return param;
                    }
                }
            }
        }
        catch (Exception e)
        {
            //Debug.LogError(e.Message);
        }
        return null;
    }

    private static int Sort(EntityParam a, EntityParam b)
    {
        if (a.type == EntityParamType.Action)
        {
            var commandParamA = a as EntityParamAction;
            var commandParamB = b as EntityParamAction;
            if (commandParamB != null)
            {
                if (commandParamA.action > commandParamB.action)
                {
                    return 1;
                }
            }
            return -1;
        }
        else if (a.type == EntityParamType.Animation)
        {
            return 1;
        }

        return 0;
    }

    public static string ToXml(EntityParam root)
    {
        root.children.Sort(Sort);

        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        doc.InsertBefore(dec, doc.DocumentElement);

        doc.AppendChild(root.ToXml(doc));

        MemoryStream ms = new MemoryStream();
        XmlTextWriter xw = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
        xw.Formatting = Formatting.Indented;
        doc.Save(xw);

        ms = (MemoryStream)xw.BaseStream;
        byte[] bytes = ms.ToArray();
        string xml = System.Text.Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

        return xml;
    }
#if UNITY_EDITOR  
    public virtual void Draw(ref Rect r)
    {
        rect = r;
    }
    public abstract bool LinkAble(INode node);

    public void OnLink(INode node)
    {
        EntityParam param = node as EntityParam;
        if(param!= null)
        {
            param.parent = this;
            children.Add(param);
        }
    }
    public void OnUnLink(INode node)
    {
        EntityParam param = node as EntityParam;
        if (param != null)
        {
            children.Remove(param);
        }
    }
   
    public virtual INode Clone(INode node)
    {
        var param = node as EntityParam;
        if(param!=null)
        {
            param.type = this.type;
            param.name = this.name;
        }
        
        return param;
    }

    public  virtual Color GetColor()
    {
        return Color.green;
    }
#endif
}

public partial class EntityParamModel : EntityParam
{
    public string model;
    public uint assetID;
    
    public float scale = 1;
    public Vector3 hitPosition = Vector3.zero;

    public EntityParamModel() { type = EntityParamType.Model; name = "Model"; }

    

#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);

        model = UnityEditor.EditorGUILayout.TextField("Model" , model);
        r.height += 20;
        assetID = (uint)Mathf.Clamp(UnityEditor.EditorGUILayout.IntField("AssetID", (int)assetID),0,uint.MaxValue);
        r.height += 20;
      
        scale = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Scale", scale), 0, float.MaxValue);
        r.height += 20;
        hitPosition = UnityEditor.EditorGUILayout.Vector3Field("HitPosition", hitPosition);
        r.height += 40;
    }
    public override bool LinkAble(INode node)
    {
        if (node.GetType() == typeof(EntityParamAnimation))
        {
            return true;
        }

        if(node.GetType() == typeof(EntityParamAction))
        {
            bool linkable = true;
            var state = node as EntityParamAction;
            for(int i = 0; i < children.Count;++i)
            {
                if(children[i].GetType() == typeof(EntityParamAction))
                {
                    var child = children[i] as EntityParamAction;
                    if(child.action == state.action)
                    {
                        linkable = false;break;
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
            param =  new EntityParamModel();
        }
        param.model = this.model;
        param.assetID = this.assetID;
     
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
        attributes.Add("model", model);
        attributes.Add("assetID", assetID.ToString());
        
        attributes.Add("scale", scale.ToString());
        attributes.Add("hitPosition", hitPosition.ToString());

        return base.ToXml(parent, attributes);
    }
    public override void ParseXml(XmlElement node)
    {
        model = node.GetAttribute("model");
        assetID = node.GetAttribute("assetID").ToUInt32Ex();
        scale = node.GetAttribute("scale").ToFloatEx();
        hitPosition = node.GetAttribute("hitPosition").ToVector3Ex();
        base.ParseXml(node);
    }
    public EntityParamAction GetAction(ActionType action)
    {
        for (int i = 0; i < children.Count; ++i)
        {
            var child =children[i] as EntityParamAction;
            if (child != null && child.action == action)
            {
                return child;
            }
        }
        return null;
    }

    
}

public partial class EntityParamAction :EntityParam
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
        duration = Mathf.Clamp( UnityEditor.EditorGUILayout.FloatField("Duration", duration), 0, float.MaxValue);
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

    public override INode Clone(INode node )
    {
        EntityParamAction param = node as EntityParamAction;
        if (param == null)
        {
            param =   new EntityParamAction();
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

public partial class EntityParamAnimation : EntityParam
{
    public string animationClip;
    public float length;
    public WrapMode mode;

    private AnimationClip mAnimationClip;
    
    public EntityParamAnimation() { type = EntityParamType.Animation; name = type.ToString(); }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);

        animationClip = UnityEditor.EditorGUILayout.TextField("AnimationClip",animationClip);
        r.height += 20;

        UnityEditor.EditorGUILayout.LabelField("Length", length.ToString());
        r.height += 20;

        mode = (WrapMode)UnityEditor.EditorGUILayout.EnumPopup("Mode", mode);
        r.height += 20;

        AnimationClip clip = (AnimationClip)UnityEditor.EditorGUILayout.ObjectField(mAnimationClip, typeof(AnimationClip), false, new GUILayoutOption[0]);
        if(clip!= null && mAnimationClip!= clip)
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
        return node.GetType().IsSubclassOf(typeof(EntityParamEffect ));
    }

    public override INode Clone(INode node)
    {
        EntityParamAnimation param = node as EntityParamAnimation;
        if(param == null)
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

/// <summary>
/// 插件参数
/// </summary>
public abstract partial class EntityParamPlugin:EntityParam
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

public partial class EntityParamPluginRun: EntityParamPlugin
{
    public EntityParamPluginRun ()
    {
        name = "RunPlugin";
        plugin = typeof(ActionPluginRun);
    }
#if UNITY_EDITOR
    public override INode Clone(INode node)
    {
        return new EntityParamPluginRun ();
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

    public List<AnimationClip> animations =new  List<AnimationClip>();

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
        else if(size < animations.Count)
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
                UnityEditor.EditorGUILayout.LabelField("  Element "+ i);
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

                if (j >= 0 && j < anims.Count && j != index )
                {
                    animations[i].animationClip = anims[j].animationClip;
                    animations[i].length = anims[j].length;
                }

                animations[i].length = UnityEditor.EditorGUILayout.FloatField("    Length", animations[i].length);
                r.height += 18;

                length += animations[i].length;

            }
            if(action!= null && action.duration != DEFAULT_DURATION)
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
            CreateXmlNode(node, "AnimationData", animation);
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


public abstract partial class EntityParamEffect :EntityParam
{
    public EffectType effectType { get; protected set; }
    public EffectArise arise;
    public EffectOn on;
 

    public uint assetID;     //资源ID
   
    public float delay;      //延迟

    public Vector3 offset;

    
    public EntityParamEffect () { type = EntityParamType.Effect; name = type.ToString(); }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);
        UnityEditor.EditorGUILayout.LabelField("EffectType", effectType.ToString());
        r.height += 20;
        arise = (EffectArise)UnityEditor.EditorGUILayout.EnumPopup("Arise", arise);
        r.height += 20;
        on = (EffectOn)UnityEditor.EditorGUILayout.EnumPopup("On", on);
        r.height += 20;
      
        assetID = (uint)UnityEditor.EditorGUILayout.IntField("AssetID", (int)assetID);
        r.height += 20;
        delay = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Delay", delay),0, float.MaxValue);
        r.height += 20;

        offset = UnityEditor.EditorGUILayout.Vector3Field("Offset", offset);
        r.height += 25;

       
    }
    public override bool LinkAble(INode node)
    {
        return node.GetType().IsSubclassOf(typeof(EntityParamEffect )) && node != this;
    }
    public override INode Clone(INode node)
    {
        EntityParamEffect  param = node as EntityParamEffect ;
        if(param!= null)
        {
            param.effectType = this.effectType;
            param.arise = this.arise;
            param.on = this.on;
          

            param.assetID = this.assetID;
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
       
        attributes.Add("assetID", assetID.ToString());    
        attributes.Add("delay", delay.ToString());
        attributes.Add("offset", offset.ToString());
       
        return base.ToXml(parent, attributes);
    }

    public override void ParseXml(XmlElement node)
    {
        effectType = node.GetAttribute("effectType").ToEnumEx<EffectType>();
        arise = node.GetAttribute("arise").ToEnumEx<EffectArise>();
        on = node.GetAttribute("on").ToEnumEx<EffectOn>();   
        assetID = node.GetAttribute("assetID").ToUInt32Ex();
        delay = node.GetAttribute("delay").ToFloatEx();
        offset = node.GetAttribute("offset").ToVector3Ex();
        base.ParseXml(node);
    }
   
}

public partial class EntityParamEffectTime :EntityParamEffect 
{
    public BoneType bone;
    public bool bind = false;//绑定
    public bool syncAnimationSpeed = false; //特效速度是否同步动作速度
    public float duration;
    public float triggerAt;

    public EntityParamEffectTime ()
    {
        effectType = EffectType.Time;
        name = effectType.ToString() + type.ToString();
    }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    { 
        base.Draw(ref r);
        duration = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Duration", duration),0, float.MaxValue);
        r.height += 20;
        triggerAt = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Trigger At", triggerAt),0, float.MaxValue);
        r.height += 20;
        bone = (BoneType)UnityEditor.EditorGUILayout.EnumPopup("Bone", bone);
        r.height += 20;
        bind = UnityEditor.EditorGUILayout.Toggle("Bind:", bind);
        r.height += 20;
        syncAnimationSpeed = UnityEditor.EditorGUILayout.Toggle("SyncAnimationSpeed", syncAnimationSpeed);
        r.height += 20;
     
    }

    public override INode Clone(INode node)
    {
        EntityParamEffectTime  param = node as EntityParamEffectTime ;
        if(param == null)
        {
            param = new EntityParamEffectTime ();
        }
        param.duration = this.duration;
        param.triggerAt = this.triggerAt;
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
        attributes.Add("triggerAt", triggerAt.ToString());
        attributes.Add("bone", bone.ToString());
        attributes.Add("bind", (bind ? 1 : 0).ToString());
        attributes.Add("syncAnimationSpeed", (syncAnimationSpeed ? 1 : 0).ToString());

        return base.ToXml(parent, attributes);
    }

    public override void ParseXml(XmlElement node)
    {
        duration = node.GetAttribute("duration").ToFloatEx();
        triggerAt = node.GetAttribute("triggerAt").ToFloatEx();
        bone = node.GetAttribute("bone").ToEnumEx<BoneType>();
        bind = node.GetAttribute("bind").ToInt32Ex() == 1;
        syncAnimationSpeed = node.GetAttribute("syncAnimationSpeed").ToInt32Ex() == 1;

        base.ParseXml(node);
    }
}

public partial class EntityParamEffectMove:EntityParamEffect 
{
    public float speed;
    public float distance;

    public Vector3 direction;

    public EntityParamEffectMove()
    {
        effectType = EffectType.Move;
        name = effectType.ToString() + type.ToString();
    }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);
        distance = Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Distance", distance), 0, float.MaxValue);
        r.height += 20;
        speed =Mathf.Clamp( UnityEditor.EditorGUILayout.FloatField("Speed", speed),0,float.MaxValue);
        r.height += 20;
        direction = UnityEditor.EditorGUILayout.Vector3Field("Direction", direction);
        r.height += 25;
    }
    public override INode Clone(INode node)
    {
        EntityParamEffectMove param = node as EntityParamEffectMove;
        if(param== null)
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

public partial class EntityParamEffectFollow :EntityParamEffect 
{
    public float speed;
    
    public EntityParamEffectFollow ()
    {
        effectType = EffectType.Follow;
        name = effectType.ToString() + type.ToString();
    }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);
        speed = Mathf.Clamp( UnityEditor.EditorGUILayout.FloatField("Speed", speed),0,float.MaxValue);
        r.height += 20;
        
    }
    public override INode Clone(INode node)
    {
        EntityParamEffectFollow  param = node as EntityParamEffectFollow ;
        if (param == null)
        {
            param = new EntityParamEffectFollow ();
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

public partial class EntityParamEffectParabola :EntityParamEffect 
{
    public float speed;      //速度
    public float gravity;    //重力（抛物线特效使用）
    public float heightOffset;
    public float heightLimit;
    public EntityParamEffectParabola ()
    {
        effectType = EffectType.Parabola;
        name = effectType.ToString() + type.ToString();
    }
#if UNITY_EDITOR
    public override void Draw(ref Rect r)
    {
        base.Draw(ref r);
        speed =Mathf.Clamp( UnityEditor.EditorGUILayout.FloatField("Speed", speed),0,float.MaxValue);
        r.height += 20;
        gravity =Mathf.Clamp(UnityEditor.EditorGUILayout.FloatField("Gravity", gravity),0,100);
        r.height += 20;
       
        heightOffset = UnityEditor.EditorGUILayout.FloatField("Height Offset", heightOffset);
        r.height += 20;
        heightLimit =Mathf.Clamp( UnityEditor.EditorGUILayout.FloatField("Height Limit", heightLimit),0,100);
        r.height += 20;
    }
    public override INode Clone(INode node)
    {
        EntityParamEffectParabola  param = node as EntityParamEffectParabola ;
        if(param == null)
        {
            param = new EntityParamEffectParabola ();
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


