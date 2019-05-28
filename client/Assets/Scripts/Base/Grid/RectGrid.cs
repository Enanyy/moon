using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectGrid <T>:Grid<T> where T:class, ITile, new ()
{
    public float tileWidth { get; private set; }
    public float tileHeight { get; private set; }
 
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
               
                Vector3 position = Vector3.zero;
                position.x = original.x + i * tileWidth;
                position.z = original.z + j * tileHeight;
               
                CreateTile(new TileIndex(j, j * columns + i, i), position);
            }
        }      
    }
  

    public TileIndex IndexOf(Vector3 position)
    {
        float x = position.x - original.x + tileWidth * 0.5f;
        float z = position.z - original.z + tileHeight * 0.5f;

        int i = (int)(x / tileWidth);

        int j = (int)(z / tileHeight);

        int index = j * columns + i;

        return new TileIndex(j,index,i);
    }

    public TileIndex IndexOf(int index)
    {
        return new TileIndex(index / columns, index, index % columns);
    }

    public TileIndex IndexOf(int line, int column)
    {
        return  new TileIndex(line, line * columns + column,column);
    }

    public T TileAt(Vector3 position)
    {
        return TileAt(IndexOf(position));
    }
    public T TileAt(int index)
    {     
        return TileAt(IndexOf(index));
    }

    public T TileAt(int line, int column)
    {
        return TileAt(IndexOf(line,column));
    }

    public List<T> TilesInRange(int index, int r)
    {
        return TilesInRange(IndexOf(index), r);
    }

    public List<T> TilesInRange(int line, int column, int r)
    {
        return TilesInRange(IndexOf(line, column),r);
    }
    
    public List<T> TilesInRange(Vector3 position,int r)
    {
        return TilesInRange(IndexOf(position), r);
    }

    public List<T> TilesInRange(TileIndex index,int r)
    {
        List<T> list = new List<T>();

        ////上下左右
        for (int i = -r; i <= r; ++i)
        {
            int middle = (index.x + i) * columns + index.z;
            if (tiles.ContainsKey(IndexOf(middle)))
            {
                int line = middle / columns;
                for (int j = -r; j <= r; ++j)
                {
                    int n = middle + j;
                    if (n / columns == line)
                    {
                        if (tiles.ContainsKey(IndexOf(n)))
                        {
                            list.Add(tiles[IndexOf(n)]);
                        }
                    }
                }
            }
        }


        return list;
    }

    public List<T> TilesInDistance(Vector3 position, int r)
    {
        return TilesInDistance(IndexOf(position), r);
    }

    public List<T> TilesInDistance(int index, int r)
    {
        return TilesInDistance(IndexOf(index), r);
    }

    public List<T> TilesInDistance(TileIndex index, int r)
    {
        List<T> list = new List<T>();
        ////上下左右
        for (int i = -r; i <= r; ++i)
        {
            int middle = (index.x + i) * columns + index.z;
            if (tiles.ContainsKey(IndexOf(middle)))
            {
                int line = middle / columns;
                for (int j = -r; j <= r; )
                {
                    int n = middle + j;
                    if (n / columns == line)
                    {
                        if (tiles.ContainsKey(IndexOf(n)))
                        {
                            list.Add(tiles[IndexOf(n)]);
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
        var indexFrom = IndexOf(from);
        var indexTo = IndexOf(to);
        return Distance(indexFrom, indexTo);
    }

    public int Distance(T from, T to)
    {
        return Distance(from.index, to.index);
    }

    public int Distance(int from, int to)
    {
        return Distance(IndexOf(from), IndexOf(from));
    }

    public int Distance(TileIndex from, TileIndex to)
    {
        return Math.Max(Math.Abs(to.x - from.x), Math.Abs(to.z - from.z));
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
