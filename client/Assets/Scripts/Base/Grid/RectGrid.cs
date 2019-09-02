using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum RectDirection
{
    Left = 0,
    Right,
    Top,
    Bottom,
    LeftTop,
    RightTop,
    LeftBottom,
    RightBottom,
}

public enum RectNeighbors
{
    All,
    Edge,
    Corner
}

public class RectGrid <T>:Grid<T> where T:class, ITile, new ()
{
    public float tileWidth { get; private set; }
    public float tileHeight { get; private set; }

    public static readonly TileIndex[] Directions = new TileIndex[]
    {
        new TileIndex(0,0,-1), //Left
        new TileIndex(0,0,1),  //Right
        new TileIndex(1,0,0),  //Top
        new TileIndex(-1,0,0), //Bottom
        new TileIndex(1,0,-1), //LeftTop
        new TileIndex(1,0,1),  //RightTop
        new TileIndex(-1,0,-1),//LeftBottom
        new TileIndex(-1,0,1), //RightBottom
    };
 
	// Use this for initialization
	public virtual void Init (Transform root, int lines, int columns,float tileWidth,float tileHeight)
    {
        this.root = root;
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
                position.x = i * tileWidth;
                position.z = j * tileHeight;
               
                CreateTile(new TileIndex(j, index, i), position);
            }
        }      
    }
  

    public TileIndex IndexOf(Vector3 position)
    {
        if (root)
        {
            Vector3 localPosition = root.InverseTransformPoint(position);

            float x = localPosition.x - tileWidth * 0.5f;
            float z = localPosition.z - tileHeight * 0.5f;

            int i = (int)(x / tileWidth);

            int j = (int)(z / tileHeight);

            int index = j * columns + i;

            return new TileIndex(j, index, i);
        }
        return new TileIndex();
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

    public List<T> Neighbours(int index)
    {
        return Neighbours(IndexOf(index));
    }

    public List<T> Neighbours(int line, int column)
    {
        return Neighbours(IndexOf(line,column));
    }
    public List<T> Neighbours(Vector3 position)
    {
        return Neighbours(IndexOf(position));
    }

    public List<T> Neighbours(T tile)
    {
        return Neighbours(tile.index);
    }

  

    public List<T> Neighbours(TileIndex index)
    {
        List<T> ret = new List<T>();
        for (int i = 0; i < Directions.Length; i++)
        {
            var tile = DirectionTo(index,(RectDirection)i, 1);
            if (tile != null)
            {
                ret.Add(tile);
            }
        }

        return ret;
    }
    public List<T> Neighbours(int index, RectNeighbors neighbor)
    {
        return Neighbours(IndexOf(index), neighbor);
    }

    public List<T> Neighbours(int line, int column, RectNeighbors neighbor)
    {
        return Neighbours(IndexOf(line, column), neighbor);
    }
    public List<T> Neighbours(Vector3 position, RectNeighbors neighbor)
    {
        return Neighbours(IndexOf(position), neighbor);
    }

    public List<T> Neighbours(T tile, RectNeighbors neighbor)
    {
        return Neighbours(tile.index, neighbor);
    }
    public List<T> Neighbours(TileIndex index,RectNeighbors neighbor)
    {
        List<T> ret = new List<T>();
        int i = 0;
        int count = Directions.Length;
        if (neighbor == RectNeighbors.Edge)
        {
            count = 4;
        }
        else if(neighbor == RectNeighbors.Corner)
        {
            i = 4;
        }

        for (i = 0; i < count ; i++)
        {
            var tile = DirectionTo(index, (RectDirection)i, 1);
            if (tile != null)
            {
                ret.Add(tile);
            }
        }

        return ret;
    }

    public T DirectionTo(int index, RectDirection direction, int distance)
    {
        return DirectionTo(IndexOf(index), direction,distance);
    }
    public T DirectionTo(int line, int column, RectDirection direction, int distance)
    {
        return DirectionTo(IndexOf(line,column), direction,distance);
    }
    public T DirectionTo(Vector3 position, RectDirection direction,int distance)
    {
        return DirectionTo(IndexOf(position), direction, distance);
    }

    public T DirectionTo(T tile, RectDirection direction, int distance)
    {
        return DirectionTo(tile.index, direction, distance);
    }

    public T DirectionTo(TileIndex index, RectDirection direction,int distance)
    {
        int i = (int) direction;
 
        TileIndex o = new TileIndex(index.x + Directions[i].x * distance,
            (index.x + Directions[i].x * distance) * columns + index.z + Directions[i].z* distance,
            index.z + Directions[i].z* distance);
        return TileAt(o);
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

        //////上下左右
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

    public static int GetCostValue(T a, T b)
    {
        int cntX = Mathf.Abs(a.index.x - b.index.x);
        int cntY = Mathf.Abs(a.index.y - b.index.y);
        // 判断到底是那个轴相差的距离更远
        if (cntX > cntY)
        {
            return 14 * cntY + 10 * (cntX - cntY);
        }
        else
        {
            return 14 * cntX + 10 * (cntY - cntX);
        }
    }
}
