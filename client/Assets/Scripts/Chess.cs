using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int index;
    public Vector3 position;

    public GameObject gameObject;

    public void Select(bool value)
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", value? Color.green:Color.white);
    }
}

public class Chess : MonoBehaviour
{

    public int lines = 16;

    public int columns = 12;

    public float nodeSize = 2.5f;

    public Dictionary<int,Node> nodes = new Dictionary<int, Node>();

    public Vector3 startPoint=Vector3.zero;

    private  Plane mPlane = new Plane(Vector3.up, Vector3.zero);
	// Use this for initialization
	void Start () {

        for (int j = 0; j < lines; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                int index = j * columns + i ;
                Node node = new Node();
                node.index = index;

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Vector3 center = transform.position;
                Vector3 position = Vector3.zero;
                position.x = startPoint.x+(i +0.5f) * nodeSize;
                position.z = startPoint.y+(j +0.5f) * nodeSize;
                go.name = index.ToString();

                go.transform.position = position;
              
                node.gameObject = go;

                nodes.Add(index,node);
            }
        }
    }

    public Node IndexOf(Vector3 position)
    {      
        if (position.x > startPoint.x + nodeSize * columns
            || position.x < startPoint.x
            || position.z > startPoint.z + nodeSize * lines
            || position.z < startPoint.z)

        {
            return null;
        }
       
        float x = position.x - startPoint.x;
        float z = position.z - startPoint.z;

        int i = (int)(x / nodeSize);

        int j = (int)(z / nodeSize);

        int index = j * columns + i;

        return IndexOf(index);
    }

    public Node IndexOf(int index)
    {
        Node node = null;
        nodes.TryGetValue(index, out node);
        return node;
    }

    public List<Node> GetCover(Vector3 position,int r)
    {
        Node node = IndexOf(position);
        if (node != null)
        {
            return GetCover(node.index,r);
        }
        return new List<Node>();
    }

    public List<Node> GetCover(int index,int r)
    {
        List<Node> covers = new List<Node>();

        ////上下左右
        for (int i = -r; i <= r; ++i)
        {
            int middle = (index / columns + i) * columns + index % columns;
            if (nodes.ContainsKey(middle))
            {
                int line = middle / columns;
                for (int j = -r; j <= r; ++j)
                {
                    int n = middle + j;
                    if (n / columns == line)
                    {
                        if (nodes.ContainsKey(n))
                        {
                            covers.Add(nodes[n]);
                        }
                    }
                }
            }
        }


        return covers;
    }

    public int Distance(Vector3 from, Vector3 to)
    {
        var fromNode = IndexOf(from);
        var toNode = IndexOf(to);
        if (fromNode != null && toNode != null)
        {
            return Distance(fromNode.index, toNode.index);
        }

        return -1;
    }

    public int Distance(int from, int to)
    {
        int fromLine = from / columns;
        int fromIndex = from % columns;

        int toLine = to / columns;
        int toIndex = to % columns;

        return Math.Max(Math.Abs(toLine - fromLine), Math.Abs(toIndex - fromIndex));
    }


    private GameObject mClick;

    private Node mNode;

    private List<Node> mCovers;
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            mPlane.Raycast(ray, out distance);
            Vector3 point = ray.GetPoint(distance);

            if (mClick == null)
            {
                mClick = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mClick.transform.localScale = Vector3.one * 0.1f;
                MeshRenderer renderer = mClick.GetComponent<MeshRenderer>();
                renderer.material.SetColor("_Color", Color.red);
            }

            mClick.transform.position = point;


            var node = IndexOf(point);
            if (node!= null)
            {
                if (mNode != null)
                {
                    mNode.Select(false);
                }

                mNode = node;
                mNode.Select(true);

                if (mCovers != null)
                {
                    for (int i = 0; i < mCovers.Count; i++)
                    {
                        mCovers[i].Select(false);
                    }
                }

                mCovers = GetCover(mNode.index, 2);
                for (int i = 0; i < mCovers.Count; i++)
                {
                    mCovers[i].Select(true);
                }
            }
        }
	}
}
