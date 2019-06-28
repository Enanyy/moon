using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

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

public enum BonePoint
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
    :ITreeNode
#endif
{
    public EntityParamType type { get; protected set; }
 
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
#if UNITY_EDITOR
        name = GetType().ToString().Replace("EntityParam","");
#endif
    }

   
   
    public virtual XmlElement ToXml(XmlNode parent, Dictionary<string, string> attributes = null)
    {
        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }

        //attributes.Add("name", name);
#if UNITY_EDITOR
        attributes.Add("rect", rect.ToStringEx());
#endif
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
#if UNITY_EDITOR
            //name = node.GetAttribute("name");
            rect = node.GetAttribute("rect").ToRectEx();
#endif

            if (node.ChildNodes != null)
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
                            AddChild(param);
                           
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
            Debug.Log(e.Message);
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
    public void AddChild(EntityParam child)
    {
        if (child == null)
        {
            return;
        }
        child.parent = this;
        children.Add(child);
#if UNITY_EDITOR
        if (OnAddChild != null)
        {
            OnAddChild(child);
        }
#endif
    }

#if UNITY_EDITOR

    public string name { get; set; }
    public Rect rect { get; set; }

    public Action<ITreeNode> OnAddChild { get; set; }

    List<ITreeNode> ITreeNode.children
    {
        get
        {
            List<ITreeNode> list = new List<ITreeNode>(children);
            
            return list;
        }
    }

    public virtual void OnDraw(ref Rect r)
    {
        rect = r;
    }
    public abstract bool ConnectableTo(ITreeNode node);

    public void OnConnect(ITreeNode node)
    {
        EntityParam param = node as EntityParam;
      
        AddChild(param);
    }
    public void OnDisconnect(ITreeNode node)
    {
        EntityParam param = node as EntityParam;
        if (param != null)
        {
            children.Remove(param);
        }
    }
   
    public virtual ITreeNode Clone(ITreeNode node)
    {
        var param = node as EntityParam;
        if(param!=null)
        {
            param.type = this.type;
            param.name = this.name;
        }
        
        return param;
    }

    public  virtual Color GetConnectionColor()
    {
        return Color.green;
    }
#endif
}


