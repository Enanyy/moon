using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int index;
    public Vector3 position;

    public GameObject gameObject;

    public Color defaultColor;

    public void Select(bool value)
    {
        int j = index;
        SetColor(value ? Color.green : defaultColor);
    }
    public void SetColor(Color color)
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", color);
    }
}



public class Grid : MonoBehaviour
{

    public int lines = 16;

    public int columns = 12;

    public float tileSize = 2.5f;

    public Dictionary<int,Tile> tiles = new Dictionary<int, Tile>();

    public Vector3 original=Vector3.zero;

    public Material material;

    private  Plane mPlane = new Plane(Vector3.up, Vector3.zero);
	// Use this for initialization
	void Start () {

        transform.position = original;

        for (int j = 0; j < lines; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                int index = j * columns + i;
                Tile tile = new Tile();
                tile.index = index;

                GameObject go = new GameObject();
                go.transform.SetParent(transform);
                MeshFilter filter = go.AddComponent<MeshFilter>();
                MeshRenderer renderer = go.AddComponent<MeshRenderer>();
                filter.mesh = GenerateMesh(tileSize);
                renderer.material = material;


                Vector3 position = Vector3.zero;
                position.x = original.x + i * tileSize;
                position.z = original.y + j * tileSize;
                go.name = index.ToString();

                go.transform.position = position;

                tile.gameObject = go;
                tile.position = position;

                if (j % 2 == 0)
                {
                    tile.defaultColor = i % 2 == 0 ? new Color(180 / 255f, 180 / 255f, 180 / 255f, 1) : new Color(90 / 255f, 90 / 255f, 90 / 255f, 1);
                }
                else
                {
                    tile.defaultColor = i % 2 == 1 ? new Color(180 / 255f, 180 / 255f, 180 / 255f, 1) : new Color(90 / 255f, 90 / 255f, 90 / 255f, 1);
                }
                tile.SetColor(tile.defaultColor);
                tiles.Add(index, tile);
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

    public static Mesh GenerateMesh(float size)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        verts.Add(new Vector3(0, 0, 0));
        verts.Add(new Vector3(0, 0, size));
        verts.Add(new Vector3(size, 0, size));
        verts.Add(new Vector3(size, 0, 0));

        tris.Add(0);
        tris.Add(1);
        tris.Add(3);

        tris.Add(3);
        tris.Add(1);
        tris.Add(2);


        uvs.Add(new Vector2(0, 0f));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
      

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.name = "Hexagonal Plane";

        mesh.RecalculateNormals();

        return mesh;
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

                //if (mCovers != null)
                //{
                //    for (int i = 0; i < mCovers.Count; i++)
                //    {
                //        mCovers[i].Select(false);
                //    }
                //}

                //mCovers =TilesInDistance (mTile.index, 2);
                //for (int i = 0; i < mCovers.Count; i++)
                //{
                //    mCovers[i].Select(true);
                //}
            }
        }
	}
}
