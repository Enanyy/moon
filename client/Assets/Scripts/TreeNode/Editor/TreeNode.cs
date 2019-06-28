using System;

using UnityEngine;

public class TreeNode
{
    public const int WIDTH = 240;
    public const int HEIGHT = 20;


    public int id { get; private set; }

 
    public string name = "Node";


    public TreeNode parent = null;


    public Rect rect;


    public ITreeNode data { get; private set; }

    public TreeNodeGraph graph { get; private set; }

    public TreeNode(TreeNodeGraph graph, int id, ITreeNode node)
    {
        this.graph = graph;
        this.id = id;
        data = node;
        name = node.name;
        rect = node.rect;
        data.OnAddChild = OnAddChild;
    }

    public void DrawWindow()
    {
        rect.height = HEIGHT;
        if (data != null)
        {
            data.OnDraw(ref rect);
        }
    }

    private void OnAddChild(ITreeNode node)
    {
        TreeNode treeNode = graph.AddNode(node);
        treeNode.parent = this;
    }
}

