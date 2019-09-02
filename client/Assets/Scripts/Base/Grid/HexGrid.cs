using UnityEngine;
using System.Collections.Generic;
using System;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public class HexGrid<T> : Grid<T> where T : class, ITile, new()
{
    //Map settings

    public HexGridShape shape { get; private set; }

    //Hex Settings
    public HexOrientation orientation { get; private set; }
    public float radius { get; private set; }


    private static readonly TileIndex[] directions =
        new TileIndex[]
        {
            new TileIndex(1, -1, 0),
            new TileIndex(1, 0, -1),
            new TileIndex(0, 1, -1),
            new TileIndex(-1, 1, 0),
            new TileIndex(-1, 0, 1),
            new TileIndex(0, -1, 1)
        };

    #region Public Methods

    public virtual void Init(Transform root, HexGridShape shape, int lines, int columns, float radius,
        HexOrientation orientation)
    {
        this.root = root;
        this.shape = shape;
        this.lines = lines;
        this.columns = columns;
        this.radius = radius;
        this.orientation = orientation;
        //Generating a new grid, clear any remants and initialise values
        Clear();

        //Generate the grid shape
        switch (shape)
        {
            case HexGridShape.Hexagon:
                GenerateHexShape();
                break;

            case HexGridShape.Rectangle:
                GenerateRectShape();
                break;
            default:
                break;
        }
    }

    public TileIndex IndexOf(int x, int y, int z)
    {
        return new TileIndex(x,y,z);
    }

    public TileIndex IndexOf(int x, int z)
    {
        return new TileIndex(x, -x - z, z);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TileIndex IndexOf(Vector3 worldPosition)
    {
        return IndexOf(worldPosition.x, worldPosition.z);
    }

    public TileIndex IndexOf(float x, float z)
    {
       
        Vector3 localPosition = root.InverseTransformPoint(new Vector3(x, 0, z));
        x = localPosition.x;
        z = localPosition.z;

        switch (orientation)
        {
            case HexOrientation.Flat:
                {
                    int q = (int)Math.Round(x / radius / 1.5f, MidpointRounding.AwayFromZero);
                    int r = (int)Math.Round(z / radius / Mathf.Sqrt(3.0f) - q * 0.5f, MidpointRounding.AwayFromZero);

                    return new TileIndex(q, r, -q - r);
                   
                }
            case HexOrientation.Pointy:
            default:
                {
                    int r = (int)Math.Round(z / radius / 1.5f, MidpointRounding.AwayFromZero);
                    int q = (int)Math.Round(x / radius / Mathf.Sqrt(3.0f) - r * 0.5f, MidpointRounding.AwayFromZero);

                    return new TileIndex(q, r, -q - r);
                    
                }
        }
    }

    public T TileAt(int x, int y, int z)
    {
        return TileAt(IndexOf(x,y,z));
    }

    public T TileAt(int x, int z)
    {
        return TileAt(IndexOf(x,z));
    }


    /// <summary>
    /// 世界坐标
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public T TileAt(Vector3 worldPosition)
    {
        return TileAt(worldPosition.x, worldPosition.z);
    }

    public T TileAt(float x, float z)
    {
        return TileAt(IndexOf(x, z));

    }

    public T DirectionTo(Vector3 position, HexDirection direction, int distance)
    {
        return DirectionTo(IndexOf(position), direction, distance);
    }
    public T DirectionTo(float x, float z, HexDirection direction, int distance)
    {
        return DirectionTo(IndexOf(x,z), direction, distance);
    }
    public T DirectionTo(T tile, HexDirection direction, int distance)
    {
        return DirectionTo(tile.index,direction,distance);
    }
    public T DirectionTo(TileIndex index, HexDirection direction, int distance)
    {
        int i = (int) direction;
        TileIndex o;
        o.x = index.x + directions[i].x * distance;
        o.y = index.y + directions[i].y * distance;
        o.z = index.z + directions[i].z * distance;
        return TileAt(o);
    }

    public  List<T> Neighbours(T tile)
    {
        List<T> ret = new List<T>();
        TileIndex o;

        for (int i = 0; i < 6; i++)
        {
            o = tile.index;
            o.x += directions[i].x;
            o.y += directions[i].y;
            o.z += directions[i].z;
            if (tiles.ContainsKey(o))
                ret.Add(tiles[o]);
        }

        return ret;
    }

    public List<T> Neighbours(TileIndex index)
    {
        return Neighbours(TileAt(index));
    }

    public List<T> Neighbours(int x, int y, int z)
    {
        return Neighbours(TileAt(x, y, z));
    }

    public List<T> Neighbours(int x, int z)
    {
        return Neighbours(TileAt(x, z));
    }

    public List<T> Neighbours(float x, float z)
    {
        return Neighbours(TileAt(x, z));
    }

    public List<T> Neighbours(Vector3 position)
    {
        return Neighbours(TileAt(position));
    }

    public List<T> TilesInRange(T center, int range)
    {
        //Return tiles rnage steps from center, http://www.redblobgames.com/grids/hexagons/#range
        List<T> ret = new List<T>();
        TileIndex o;

        for (int dx = -range; dx <= range; dx++)
        {
            int i = Mathf.Max(-range, -dx - range);
            int j = Mathf.Min(range, -dx + range);
            for (int dy = i; dy <= j; dy++)
            {
                o = new TileIndex(dx, dy, -dx - dy);
                o.x += center.index.x;
                o.y += center.index.y;
                o.z += center.index.z;

                if (tiles.ContainsKey(o))
                    ret.Add(tiles[o]);
            }
        }

        return ret;
    }



    public List<T> TilesInRange(TileIndex index, int range)
    {
        return TilesInRange(TileAt(index), range);
    }

    public List<T> TilesInRange(int x, int y, int z, int range)
    {
        return TilesInRange(TileAt(x, y, z), range);
    }

    public List<T> TilesInRange(int x, int z, int range)
    {
        return TilesInRange(TileAt(x, z), range);
    }

    public List<T> TilesInDistance(T center, int range)
    {
        List<T> ret = new List<T>();
        TileIndex o;

        for (int dx = -range; dx <= range; dx++)
        {
            int i = Mathf.Max(-range, -dx - range);
            int j = Mathf.Min(range, -dx + range);
            for (int dy = i; dy <= j;)
            {
                o = new TileIndex(dx, dy, -dx - dy);
                o.x += center.index.x;
                o.y += center.index.y;
                o.z += center.index.z;

                if (tiles.ContainsKey(o))
                    ret.Add(tiles[o]);

                dy = (dx == -range || dx == range) ? dy + 1 : dy + (j - i);
            }
        }

        return ret;
    }

    public List<T> TilesInDistance(TileIndex index, int range)
    {
        return TilesInDistance(TileAt(index), range);
    }

    public List<T> TilesInDistance(int x, int y, int z, int range)
    {
        return TilesInDistance(TileAt(x, y, z), range);
    }

    public List<T> TilesInDistance(int x, int z, int range)
    {
        return TilesInDistance(TileAt(x, z), range);
    }

    public int Distance(TileIndex a, TileIndex b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
    }

    public int Distance(T a, T b)
    {
        return Distance(a.index, b.index);
    }

    public int Distance(Vector3 from, Vector3 to)
    {
        return Distance(IndexOf(from), IndexOf(to));
    }

    #endregion

    #region Private Methods

    private void GenerateHexShape()
    {
        Vector3 pos = Vector3.zero;

        for (int q = -lines; q <= lines; q++)
        {
            int r1 = Mathf.Max(-columns, -q - columns);
            int r2 = Mathf.Min(columns, -q + columns);
            for (int r = r1; r <= r2; r++)
            {
                switch (orientation)
                {
                    case HexOrientation.Flat:
                        pos.x = radius * 3.0f / 2.0f * q;
                        pos.z = radius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                        break;

                    case HexOrientation.Pointy:
                        pos.x = radius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                        pos.z = radius * 3.0f / 2.0f * r;
                        break;
                }

                var index = new TileIndex(q, r, -q - r);
                CreateTile(index, pos);

            }
        }
    }

    private void GenerateRectShape()
    {
        Vector3 pos = Vector3.zero;

        switch (orientation)
        {
            case HexOrientation.Flat:
                for (int q = 0; q < lines; q++)
                {
                    int qOff = q >> 1;
                    for (int r = -qOff; r < columns - qOff; r++)
                    {
                        pos.x = radius * 3.0f / 2.0f * q;
                        pos.z = radius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);

                        var index = new TileIndex(q, r, -q - r);
                        CreateTile(index, pos);
                    }
                }

                break;

            case HexOrientation.Pointy:
                for (int r = 0; r < columns; r++)
                {
                    int rOff = r >> 1;
                    for (int q = -rOff; q < lines - rOff; q++)
                    {
                        pos.x = radius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                        pos.z = radius * 3.0f / 2.0f * r;

                        var index = new TileIndex(q, r, -q - r);
                        CreateTile(index, pos);
                    }
                }

                break;
        }
    }



    #endregion

    #region  Static Methods

    public static Vector3 Corner(Vector3 origin, float radius, int corner, HexOrientation orientation)
    {
        float angle = 60 * corner;
        if (orientation == HexOrientation.Pointy)
            angle += 30;
        angle *= Mathf.PI / 180;
        return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
    }

    public static Mesh GenerateHexMesh(float radius, HexOrientation orientation)
    {
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < 6; i++)
            verts.Add(Corner(Vector3.zero, radius, i, orientation));

        tris.Add(0);
        tris.Add(2);
        tris.Add(1);

        tris.Add(0);
        tris.Add(5);
        tris.Add(2);

        tris.Add(2);
        tris.Add(5);
        tris.Add(3);

        tris.Add(3);
        tris.Add(5);
        tris.Add(4);

        //UVs are wrong, I need to find an equation for calucalting them
        uvs.Add(new Vector2(0.5f, 1f));
        uvs.Add(new Vector2(1, 0.75f));
        uvs.Add(new Vector2(1, 0.25f));
        uvs.Add(new Vector2(0.5f, 0));
        uvs.Add(new Vector2(0, 0.25f));
        uvs.Add(new Vector2(0, 0.75f));

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.name = "Hexagonal Plane";

        mesh.RecalculateNormals();

        return mesh;
    }

    public static int GetCostValue(T a, T b)
    {
        int cntX = Mathf.Abs(a.index.x - b.index.x);
        int cntZ = Mathf.Abs(a.index.z - b.index.z);
        // 判断到底是那个轴相差的距离更远
        if (cntX > cntZ)
        {
            return 14 * cntZ + 10 * (cntX - cntZ);
        }
        else
        {
            return 14 * cntX + 10 * (cntZ - cntX);
        }
    }
    #endregion


}

[System.Serializable]
public enum HexGridShape {
	Rectangle,
	Hexagon,
}

[System.Serializable]
public enum HexOrientation {
	Pointy,
	Flat
}