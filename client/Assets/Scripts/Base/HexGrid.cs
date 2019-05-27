using UnityEngine;
using System.Collections.Generic;
using System;

public interface IHexTile
{
    TileIndex index { get; set; }
    Vector3 position { get; set; }
    void Clear();
}


[System.Serializable]
public struct TileIndex
{
    public int x;
    public int y;
    public int z;

    public TileIndex(int x, int y, int z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public TileIndex(int x, int z)
    {
        this.x = x; this.z = z; this.y = -x - z;
    }

    public static TileIndex operator +(TileIndex a, TileIndex b)
    {
        return new TileIndex(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static TileIndex operator *(TileIndex a, int distance)
    {
        a.x *= distance;
        a.z *= distance;
        a.y = -a.x - a.z;
        return a;
    }

    public static bool operator ==(TileIndex a, TileIndex b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(TileIndex a, TileIndex b)
    {
        return !a.Equals(b);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        TileIndex o = (TileIndex)obj;
        if ((object)o == null)
            return false;
        return ((x == o.x) && (y == o.y) && (z == o.z));
    }

    public bool Equals(TileIndex o)
    {
        return ((x == o.x) && (y == o.y) && (z == o.z));
    }

    public override int GetHashCode()
    {
        return (x.GetHashCode() ^ (y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5)) / 2) + (x.GetHashCode() << 6) + (x.GetHashCode() >> 2)));
    }

    public override string ToString()
    {
        return string.Format("[{0},{1},{2}]", x, y, z);
    }
}
public class HexGrid<T> where T : class, IHexTile, new()
{
    //Map settings


    public GridShape gridShape = GridShape.Rectangle;
    public int gridWidth;
    public int gridHeight;
    /// <summary>
    /// GridShape.Rectangle的起始位置或GridShape.Hexagon的中心点
    /// </summary>

    public Vector3 center = Vector3.zero;

    //Hex Settings
    public HexOrientation hexOrientation = HexOrientation.Flat;
    public float hexRadius = 1;
  

    private static TileIndex[] directions =
        new TileIndex[]
        {
            new TileIndex(1, -1, 0),
            new TileIndex(1, 0, -1),
            new TileIndex(0, 1, -1),
            new TileIndex(-1, 1, 0),
            new TileIndex(-1, 0, 1),
            new TileIndex(0, -1, 1)
        };



    #region Getters and Setters

    public Dictionary<TileIndex, T> Tiles { get; } = new Dictionary<TileIndex, T>();

    #endregion

    #region Public Methods

    public void GenerateGrid()
    {
        //Generating a new grid, clear any remants and initialise values
        Clear();

        //Generate the grid shape
        switch (gridShape)
        {
            case GridShape.Hexagon:
                GenerateHexShape();
                break;

            case GridShape.Rectangle:
                GenerateRectShape();
                break;
            default:
                break;
        }
    }
    public virtual void Clear()
    {
        foreach (var v in Tiles)
        {
            v.Value.Clear();
        }
        Tiles.Clear();
    }

    public T TileAt(TileIndex index)
    {
        if (Tiles.ContainsKey(index))
            return Tiles[index];
        return null;
    }

    public T TileAt(int x, int y, int z)
    {
        return TileAt(new TileIndex(x, y, z));
    }

    public T TileAt(int x, int z)
    {
        return TileAt(new TileIndex(x, z));
    }

    /// <summary>
    /// 世界坐标
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public T TileAt(Vector3 position)
    {
        return TileAt(position.x, position.z);
    }

    public T TileAt(float x, float z)
    {
        x -= center.x;
        z -= center.z;

        switch (hexOrientation)
        {
            case HexOrientation.Flat:
            {
                int q = (int) Math.Round(x / hexRadius / 1.5f, MidpointRounding.AwayFromZero);
                int r = (int) Math.Round(z / hexRadius / Mathf.Sqrt(3.0f) - q * 0.5f, MidpointRounding.AwayFromZero);

                var index = new TileIndex(q, r, -q - r);
                return TileAt(index);
            }
            case HexOrientation.Pointy:
            {
                int r = (int) Math.Round(z / hexRadius / 1.5f, MidpointRounding.AwayFromZero);
                int q = (int) Math.Round(x / hexRadius / Mathf.Sqrt(3.0f) - r * 0.5f, MidpointRounding.AwayFromZero);

                var index = new TileIndex(q, r, -q - r);
                return TileAt(index);
            }
        }

        return null;
    }



    public List<T> Neighbours(T tile)
    {
        List<T> ret = new List<T>();
        TileIndex o;

        for (int i = 0; i < 6; i++)
        {
            o = tile.index + directions[i];
            if (Tiles.ContainsKey(o))
                ret.Add(Tiles[o]);
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
            for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                o = new TileIndex(dx, dy, -dx - dy) + center.index;
                if (Tiles.ContainsKey(o))
                    ret.Add(Tiles[o]);
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
        List<T> ret = TilesInRange(center, range);
        ret.Remove(center);
        if (range > 1)
        {
            List<T> insides = TilesInRange(center, range - 1);
            for (int i = 0; i < insides.Count; ++i)
            {
                ret.Remove(insides[i]);
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



    #endregion

    #region Private Methods

    private void GenerateHexShape()
    {
        Vector3 pos = Vector3.zero;

        int mapSize = Mathf.Max(gridWidth, gridHeight);

        for (int q = -mapSize; q <= mapSize; q++)
        {
            int r1 = Mathf.Max(-mapSize, -q - mapSize);
            int r2 = Mathf.Min(mapSize, -q + mapSize);
            for (int r = r1; r <= r2; r++)
            {
                switch (hexOrientation)
                {
                    case HexOrientation.Flat:
                        pos.x = hexRadius * 3.0f / 2.0f * q;
                        pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                        break;

                    case HexOrientation.Pointy:
                        pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                        pos.z = hexRadius * 3.0f / 2.0f * r;
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

        switch (hexOrientation)
        {
            case HexOrientation.Flat:
                for (int q = 0; q < gridWidth; q++)
                {
                    int qOff = q >> 1;
                    for (int r = -qOff; r < gridHeight - qOff; r++)
                    {
                        pos.x = hexRadius * 3.0f / 2.0f * q;
                        pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);

                        var index = new TileIndex(q, r, -q - r);
                        CreateTile(index, pos);
                    }
                }

                break;

            case HexOrientation.Pointy:
                for (int r = 0; r < gridHeight; r++)
                {
                    int rOff = r >> 1;
                    for (int q = -rOff; q < gridWidth - rOff; q++)
                    {
                        pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                        pos.z = hexRadius * 3.0f / 2.0f * r;

                        var index = new TileIndex(q, r, -q - r);
                        CreateTile( index, pos);
                    }
                }

                break;
        }
    }

    protected virtual T CreateTile(TileIndex index, Vector3 position)
    {
        T tile = new T();

        Tiles.Add(tile.index, tile);
        return tile;
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

    public static void GenerateHexMesh(float radius, HexOrientation orientation, ref Mesh mesh)
    {
        mesh = new Mesh();

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
    }


    #endregion
}

[System.Serializable]
public enum GridShape {
	Rectangle,
	Hexagon,
}

[System.Serializable]
public enum HexOrientation {
	Pointy,
	Flat
}