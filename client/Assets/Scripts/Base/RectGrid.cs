using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITile
{
    int index { get; set; }
    int line { get; set; }
    int column { get; set; }
    Vector3 position { get; set; }
}



public class RectGrid <T> where T:class, ITile, new ()
{
    public int lines { get; private set; }

    public int columns { get; private set; }

    public float tileWidth { get; private set; }
    public float tileHeight { get; private set; }

    public Dictionary<int, T> tiles { get; } = new Dictionary<int, T>();

    public Vector3 original { get; private set; }

	// Use this for initialization
	public virtual void Init (Vector3 original, int lines, int columns,float tileWidth,float tileHeight)
    {
        this.original = original;
        this.lines = lines;
        this.columns = columns;
        this.tileWidth = tileWidth;
        this.tileHeight = tileHeight;

        for (int j = 0; j < lines; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                int index = j * columns + i;
                T tile = new T();
                tile.index = index;
                tile.line = j;
                tile.column = i;

                Vector3 position = Vector3.zero;
                position.x = original.x + i * tileWidth;
                position.z = original.z + j * tileHeight;

                tile.position = position;

                tiles.Add(index, tile);

                OnCreateTile(tile);
            }
        }      
    }

    protected virtual void OnCreateTile(T t)
    {

    }

    public T TileAt(Vector3 position)
    {
        if (position.x > original.x + tileWidth * (columns - 0.5f)
            || position.x < original.x - tileWidth * 0.5f
            || position.z > original.z + tileHeight * (lines - 0.5f)
            || position.z < original.z - tileHeight * 0.5f)

        {
            return null;
        }
       
        float x = position.x - original.x + tileWidth *0.5f;
        float z = position.z - original.z + tileHeight * 0.5f;

        int i = (int)(x / tileWidth);

        int j = (int)(z / tileHeight);

        int index = j * columns + i;

        return TileAt(index);
    }

    public T TileAt(int index)
    {
        T tile = null;
        tiles.TryGetValue(index, out tile);
        return tile;
    }

    public T TileAt(int line, int column)
    {
        int index = line * columns + column;
        return TileAt(index);
    }

    public List<T> TilesInRange(Vector3 position,int r)
    {
        T tile = TileAt(position);
        if (tile != null)
        {
            return TilesInRange(tile.index,r);
        }
        return new List<T>();
    }

    public List<T> TilesInRange(int index,int r)
    {
        List<T> list = new List<T>();

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
    public List<T> TilesInDistance(Vector3 position, int r)
    {
        T tile = TileAt(position);
        if (tile != null)
        {
            return TilesInDistance(tile.index, r);
        }
        return new List<T>();
    }

    public List<T> TilesInDistance(int index, int r)
    {
        List<T> list = new List<T>();
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
        var fromNode = TileAt(from);
        var toNode = TileAt(to);
        if (fromNode != null && toNode != null)
        {
            return Distance(fromNode.index, toNode.index);
        }

        return -1;
    }

    public int Distance(T from, T to)
    {
        return Distance(from.index, to.index);
    }

    public int Distance(int from, int to)
    {
        int fromLine = from / columns;
        int fromIndex = from % columns;

        int toLine = to / columns;
        int toIndex = to % columns;

        return Math.Max(Math.Abs(toLine - fromLine), Math.Abs(toIndex - fromIndex));
    }

    public static Mesh GenerateTileMesh(float tileWidth,float tileHeight)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        verts.Add(new Vector3(-0.5f * tileWidth, 0, -0.5f * tileHeight));
        verts.Add(new Vector3(-0.5f * tileWidth, 0, 0.5f * tileHeight));
        verts.Add(new Vector3(0.5f * tileWidth, 0, 0.5f * tileHeight));
        verts.Add(new Vector3(0.5f * tileWidth, 0, -0.5f * tileHeight));

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

        mesh.name = "Tile";

        mesh.RecalculateNormals();

        return mesh;
    }
}
