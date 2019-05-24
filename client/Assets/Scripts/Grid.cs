using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
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



public class Grid : MonoBehaviour
{

    public int lines = 16;

    public int columns = 12;

    public float tileSize = 2.5f;

    public Dictionary<int,Tile> tiles = new Dictionary<int, Tile>();

    public Vector3 original=Vector3.zero;

    private  Plane mPlane = new Plane(Vector3.up, Vector3.zero);
	// Use this for initialization
	void Start () {

        for (int j = 0; j < lines; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                int index = j * columns + i ;
                Tile tile = new Tile();
                tile.index = index;

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Vector3 center = transform.position;
                Vector3 position = Vector3.zero;
                position.x = original.x+(i +0.5f) * tileSize;
                position.z = original.y+(j +0.5f) * tileSize;
                go.name = index.ToString();

                go.transform.position = position;
              
                tile.gameObject = go;

                tiles.Add(index,tile);
            }
        }
    }

    public Tile IndexOf(Vector3 position)
    {      
        if (position.x > original.x + tileSize * columns
            || position.x < original.x
            || position.z > original.z + tileSize * lines
            || position.z < original.z)

        {
            return null;
        }
       
        float x = position.x - original.x;
        float z = position.z - original.z;

        int i = (int)(x / tileSize);

        int j = (int)(z / tileSize);

        int index = j * columns + i;

        return IndexOf(index);
    }

    public Tile IndexOf(int index)
    {
        Tile node = null;
        tiles.TryGetValue(index, out node);
        return node;
    }

    public List<Tile> TilesInRange(Vector3 position,int r)
    {
        Tile tile = IndexOf(position);
        if (tile != null)
        {
            return TilesInRange(tile.index,r);
        }
        return new List<Tile>();
    }

    public List<Tile> TilesInRange(int index,int r)
    {
        List<Tile> list = new List<Tile>();

        ////上下左右
        for (int i = -r; i <= r; ++i)
        {
            int middle = (index / columns + i) * columns + index % columns;
            if (tiles.ContainsKey(middle))
            {
                int line = middle / columns;
                for (int j = -r; j <= r; ++j)
                {
                    int n = middle + j;
                    if (n / columns == line)
                    {
                        if (tiles.ContainsKey(n))
                        {
                            list.Add(tiles[n]);
                        }
                    }
                }
            }
        }


        return list;
    }
    public List<Tile> TilesInDistance(Vector3 position, int r)
    {
        Tile tile = IndexOf(position);
        if (tile != null)
        {
            return TilesInDistance(tile.index, r);
        }
        return new List<Tile>();
    }

    public List<Tile> TilesInDistance(int index, int r)
    {
        List<Tile> list = new List<Tile>();
        ////上下左右
        for (int i = -r; i <= r; ++i)
        {
            int middle = (index / columns + i) * columns + index % columns;
            if (tiles.ContainsKey(middle))
            {
                int line = middle / columns;
                for (int j = -r; j <= r; )
                {
                    int n = middle + j;
                    if (n / columns == line)
                    {
                        if (tiles.ContainsKey(n))
                        {
                            list.Add(tiles[n]);
                        }
                    }

                    j = (i == -r || i == r) ? j + 1 : j + r * 2;
                }
            }
        }

        return list;
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

    private Tile mTile;

    private List<Tile> mCovers;
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


            var tile = IndexOf(point);
            if (tile!= null)
            {
                if (mTile != null)
                {
                    mTile.Select(false);
                }

                mTile = tile;
                mTile.Select(true);

                if (mCovers != null)
                {
                    for (int i = 0; i < mCovers.Count; i++)
                    {
                        mCovers[i].Select(false);
                    }
                }

                mCovers =TilesInDistance (mTile.index, 2);
                for (int i = 0; i < mCovers.Count; i++)
                {
                    mCovers[i].Select(true);
                }
            }
        }
	}
}
