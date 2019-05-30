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

public abstract class Grid<T> where T: class, ITile,new()
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

    #region FindPath

    private class PathNode
    {
        public T tile;
        public int h;
        public int g;
        public int f
        {
            get { return h + g; }
        }
        public T parent = null;
    }

    private List<T> mOpenList = new List<T>();
    private List<T> mCloseList = new List<T>();
    private Dictionary<T, PathNode> mNodeDic = new Dictionary<T, PathNode>();

    public Stack<T> FindPath(T from, T to, Func<T, bool> isValid, Func<T, List<T>> getNeihbors,
        Func<T, T, int> getCostValue)
    {
        Stack<T> result = new Stack<T>();

        if (from == null || to == null || isValid == null || getNeihbors == null || getCostValue == null)
        {
            Debug.LogError("参数不能为空");
            return result;
        }

        mOpenList.Clear();
        mCloseList.Clear();
        mNodeDic.Clear();


        //将起点作为待处理的点放入开启列表，
        mOpenList.Add(from);

        //如果开启列表没有待处理点表示寻路失败，此路不通
        while (mOpenList.Count > 0)
        {
            //遍历开启列表，找到消费最小的点作为检查点
            T cur = mOpenList[0];

            if (mNodeDic.ContainsKey(cur) == false)
            {
                mNodeDic.Add(cur, new PathNode
                {
                    h = 0,
                    g = 0,
                    tile = cur,
                    parent = null,
                });
            }

            var curNode = mNodeDic[cur];

            for (int i = 0; i < mOpenList.Count; i++)
            {
                var t = mOpenList[i];
                if (mNodeDic.ContainsKey(t) == false)
                {
                    mNodeDic.Add(t, new PathNode
                    {
                        h = 0,
                        g = 0,
                        tile = t,
                        parent = null,
                    });
                }

                var node = mNodeDic[t];

                if (node.f < curNode.f && node.h < curNode.h)
                {
                    cur = mOpenList[i];
                    curNode = node;
                }
            }


            //从开启列表中删除检查点，把它加入到一个“关闭列表”，列表中保存所有不需要再次检查的方格。
            mOpenList.Remove(cur);
            mCloseList.Add(cur);

            //检查是否找到终点
            if (cur == to)
            {
                var tile = cur;
                while (tile != null)
                {
                    result.Push(tile);
                    if (mNodeDic.ContainsKey(tile))
                    {
                        tile = mNodeDic[tile].parent;
                    }
                    else
                    {
                        tile = null;
                    }
                }

                break;
            }

            ////根据检查点来找到周围可行走的点
            //1.如果是墙或者在关闭列表中则跳过
            //2.如果点不在开启列表中则添加
            //3.如果点在开启列表中且当前的总花费比之前的总花费小，则更新该点信息
            List<T> neighbours = getNeihbors(cur);
            for (int i = 0; i < neighbours.Count; i++)
            {
                var neighbour = neighbours[i];

                if (isValid(neighbour) == false || mCloseList.Contains(neighbour))
                    continue;

                int cost = curNode.g + getCostValue(neighbour, cur);

                if (mNodeDic.ContainsKey(neighbour) == false)
                {
                    mNodeDic.Add(neighbour, new PathNode
                    {
                        tile = neighbour,
                        g = 0,
                        h = 0,
                        parent = null,
                    });
                }

                var neighborNode = mNodeDic[neighbour];

                if (cost < neighborNode.g || !mOpenList.Contains(neighbour))
                {
                    neighborNode.g = cost;
                    neighborNode.h = getCostValue(neighbour, to);
                    neighborNode.parent = cur;

                    if (!mOpenList.Contains(neighbour))
                    {
                        mOpenList.Add(neighbour);
                    }
                }
            }
        }

        return result;
    }
    #endregion
}

