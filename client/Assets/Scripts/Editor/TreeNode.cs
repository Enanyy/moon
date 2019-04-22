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


    public INode data { get; private set; }

    public TreeNode( int id, INode node)
    {
        this.id = id;
        data = node;
        name = node.name;
        rect = node.rect;
    }

    public void DrawWindow()
    {
        rect.height = HEIGHT;
        if (data != null)
        {
            data.Draw(ref rect);
        }
    }
}

