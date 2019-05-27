using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITile
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

public class Grid<T> where T: class, ITile,new()
{
    public int lines { get; protected set; }
    public int columns { get; protected set; }

    public Vector3 original { get; protected set; }

    public Dictionary<TileIndex, T> tiles { get; } = new Dictionary<TileIndex, T>();

    protected virtual T CreateTile(TileIndex index, Vector3 position)
    {
        T tile = new T();
        tile.index = index;
        tile.position = position;

        tiles.Add(tile.index, tile);
        return tile;
    }

    public T TileAt(TileIndex index)
    {
        T tile = null;
        tiles.TryGetValue(index, out tile);
        return tile;
    }

    public virtual void Clear()
    {
        foreach (var v in tiles)
        {
            v.Value.Clear();
        }
        tiles.Clear();
    }
}

