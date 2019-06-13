using UnityEngine;
using System;
using System.Collections.Generic;

public class TreeNodeGraph
{
    public List<TreeNode> nodes { get; private set; }
    public TreeNodeGraph()
    {
        nodes = new List<TreeNode>();
    }
    public int GeneratorID()
    {
        int id = 0;
        for (int i = 0; i < nodes.Count; ++i)
        {
            id = id == -1 || nodes[i].id > id ? nodes[i].id : id;
        }
        return id + 1;
    }

    public TreeNode AddNode(INode node)
    {
        TreeNode treeNode = new TreeNode(this, GeneratorID(), node);
        nodes.Add(treeNode);
        return treeNode;
    }

    public TreeNode GetNode(int id)
    {
        if (nodes == null)
        {
            return null;
        }
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (nodes[i].id == id)
            {
                return nodes[i];
            }
        }
        return null;
    }

    public TreeNode GetNode(Vector3 position)
    {
        for(int i = 0; i < nodes.Count; ++i)
        {
            if(nodes[i].rect.Contains(position))
            {
                return nodes[i];
            }
        }
        return null;
    }

    public void RemoveNode(TreeNode node)
    {
        if(node!= null)
        {
            if(node.parent!= null)
            {
                node.parent.data.OnUnLink(node.data);
            }
           
            nodes.Remove(node);

            var list = GetByParent(node);
            for(int  i = 0; i < list.Count; ++i)
            {
                list[i].parent = null;
            }
        }
    }

    public List<TreeNode> GetByParent(TreeNode parent)
    {
        List<TreeNode> list = new List<TreeNode>();
        if(parent!=null)
        {
            for(int i = 0; i< nodes.Count;++ i)
            {
                if(nodes[i].parent == parent)
                {
                    list.Add(nodes[i]);
                }
            }
        }
        return list;
    }

    public TreeNode GetRoot()
    {
        for(int i = 0; i < nodes.Count; ++i)
        {
            if(nodes[i].parent == null)
            {
                return nodes[i];
            }
        }
        return null;
    }
}
