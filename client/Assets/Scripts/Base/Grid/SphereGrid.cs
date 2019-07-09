using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;



public class Tile
{
    public int index;
    /// <summary>
    /// localPosition
    /// </summary>
    public Vector3 a { get; private set; }
    /// <summary>
    /// localPosition
    /// </summary>
    public Vector3 b { get; private set; }
    /// <summary>
    /// localPosition
    /// </summary>
    public Vector3 c { get; private set; }
    /// <summary>
    /// localPosition
    /// </summary>
    public Vector3 center { get; private set; }
    /// <summary>
    /// 三角网格
    /// </summary>
    public SphereGrid grid { get; private set; }

    /// <summary>
    /// 邻边三角形（不包括自己）
    /// </summary>
    public List<Tile> neihbors { get; private set; }

    /// <summary>
    /// 每个顶点对应的三角形(不包含自己)
    /// </summary>
    public Dictionary<Vector3, List<Tile>> corners { get; private set; }
    /// <summary>
    /// 父三角形
    /// </summary>
    public Tile parent { get; private set; }
    /// <summary>
    /// 所有三角形
    /// </summary>
    public List<Tile> children { get; private set; }

    public GameObject go { get; private set; }

    public bool isValid = true;

    /// <summary>
    ///获取根三角形，也就是属于20个面的哪个
    /// </summary>
    public Tile root
    {
        get
        {
            Tile tile = this;
            while (tile.parent != null)
            {
                tile = tile.parent;
            }

            return tile;
        }
    }

    public Tile(SphereGrid grid, Tile parent)
    {
        this.grid = grid;
        this.parent = parent;
    }

    public Tile(SphereGrid grid, Tile parent, Vector3 a, Vector3 b, Vector3 c)
    {
        this.grid = grid;
        this.parent = parent;
        this.a = a;
        this.b = b;
        this.c = c;
        Debug.Assert(a != b);
        Debug.Assert(b != c);
        Debug.Assert(a != c);
        center = (a + b + c) / 3f;
        neihbors = new List<Tile>();
        children = new List<Tile>();
        corners = new Dictionary<Vector3, List<Tile>>();
    }

    #region 可视化相关
    public void Show(Material material)
    {
        if (grid.root)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            verts.Add(a);
            verts.Add(b);
            verts.Add(c);

            tris.Add(0);
            tris.Add(1);
            tris.Add(2);


            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0.5f, 1));
            uvs.Add(new Vector2(1, 0));

            Mesh ms = new Mesh();
            ms.vertices = verts.ToArray();
            ms.triangles = tris.ToArray();
            ms.uv = uvs.ToArray();

            ms.RecalculateBounds();
            ms.RecalculateNormals();

            go = new GameObject();
            go.name = index.ToString();
            go.transform.SetParent(grid.root);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;
            var mr = go.AddComponent<MeshRenderer>();
            var mf = go.AddComponent<MeshFilter>();

            mf.mesh = ms;
            mr.material = material;
            mr.enabled = true;
        }
    }

    public Color defaultColor;
    public void SetColor(Color color)
    {
        if (go != null)
        {
            var r = go.GetComponent<MeshRenderer>();
            r.material.SetColor("_Color", color);
        }
    }

    public void SetActive(bool active)
    {
        if (go != null)
        {
            var r = go.GetComponent<MeshRenderer>();
            r.enabled = active;
        }
    }
    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public bool Contains(Vector3 worldPosition)
    {
        Vector3 worldA = grid.root.TransformPoint(a);
        Vector3 worldB = grid.root.TransformPoint(b);
        Vector3 worldC = grid.root.TransformPoint(c);
        return PointInTriangle(worldA, worldB, worldC, worldPosition);
    }
    /// <summary>
    /// 判断点是否在三角形内
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static bool PointInTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
    {

        Vector3 sa = a - p; //new Vector2(a.x - p.x, a.z - p.z);
        Vector3 sb = b - p; //new Vector2(b.x - p.x, b.z - p.z);
        Vector3 sc = c - p;// new Vector2(c.x - p.x, c.z - p.z);

        float angle1 = Mathf.Abs(Vector3.Angle(sa, sb));
        float angle2 = Mathf.Abs(Vector3.Angle(sb, sc));
        float angle3 = Mathf.Abs(Vector3.Angle(sc, sa));
        if (angle1 + angle2 < 180 || angle2 + angle3 < 180 || angle3 + angle1 < 180)
        {
            return false;
        }

        return true;
    }

    public XmlElement ToXml(XmlNode parent)
    {
        Dictionary<string, string> attributes = new Dictionary<string, string>();

        attributes.Add("index", index.ToString());
        attributes.Add("a", a.ToStringEx());
        attributes.Add("b", b.ToStringEx());
        attributes.Add("c", c.ToStringEx());
        if (isValid == false)
        {
            attributes.Add("isValid", "0");
        }

        var node = CreateXmlNode(parent, GetType().ToString(), attributes);

        for (int i = 0; i < children.Count; ++i)
        {
            children[i].ToXml(node);
        }

        return node;
    } 


    public static XmlElement CreateXmlNode(XmlNode parent, string tag, Dictionary<string, string> attributes)
    {
        XmlDocument doc;
        if (parent.ParentNode == null)
        {
            doc = (XmlDocument)parent;
        }
        else
        {
            doc = parent.OwnerDocument;
        }
        XmlElement node = doc.CreateElement(tag);

        parent.AppendChild(node);

        foreach (var v in attributes)
        {
            //创建一个属性
            XmlAttribute attribute = doc.CreateAttribute(v.Key);
            attribute.Value = v.Value;
            //xml节点附件属性
            node.Attributes.Append(attribute);
        }

        return node;
    }


    public virtual void ParseXml(XmlElement node)
    {
        if (node != null)
        {
            index = node.GetAttribute("index").ToInt32Ex();
            a = node.GetAttribute("a").ToVector3Ex();
            b = node.GetAttribute("b").ToVector3Ex();
            c = node.GetAttribute("c").ToVector3Ex();
            isValid = node.GetAttribute("isValid") == "1";
            if (node.ChildNodes != null)
            {
                for (int i = 0; i < node.ChildNodes.Count; ++i)
                {
                    var child = node.ChildNodes[i] as XmlElement;
                    Type type = Type.GetType(child.Name);
                    if (type == GetType())
                    {
                        Tile tile = new Tile(grid,this);
                        tile.ParseXml(child);

                        children.Add(tile);
                        grid.tiles.Add(tile);
                    }
                }
            }
        }
    }

}
public class SphereGrid
{
    /// <summary>
    /// 所有最小三角面
    /// </summary>
    public List<Tile> tiles = new List<Tile>();
    /// <summary>
    /// 球半径
    /// </summary>
    public float radius;
    /// <summary>
    /// 递归细分次数
    /// </summary>
    public int recursion;
    /// <summary>
    /// 20个三角面的树结构
    /// </summary>
    public List<Tile> roots = new List<Tile>();
    /// <summary>
    /// 球的Transform，SphereCollider，用于点击定位球面位置已及三角形的顶点转换为世界坐标
    /// </summary>
    public Transform root { get; private set; }

    public SphereGrid()
    {
    }

    public void SetRoot(Transform root)
    {
        this.root = root;

        SphereCollider collider = root.GetComponent<SphereCollider>();
        if (collider == null) collider = root.gameObject.AddComponent<SphereCollider>();
        collider.center = Vector3.zero;
        collider.radius = radius;
    }
    /// <summary>
    /// 初始化球形三角网格
    /// </summary>
    /// <param name="radius">球形半径</param>
    /// <param name="recursion">递归细分次数</param>
    public void Init(float radius, int recursion)
    {
        this.radius = radius;
        this.recursion = recursion;

        if (recursion < 1) recursion = 1;

        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

        vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

        vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);


        // create 20 triangles of the icosahedron
        List<TriangleIndices> faces = new List<TriangleIndices>();
        List<TriangleIndices> faces2 = new List<TriangleIndices>();

        // 5 faces around point 0
        faces.Add(new TriangleIndices(null, new Vector3Int(0, 11, 5)));
        faces.Add(new TriangleIndices(null, new Vector3Int(0, 5, 1)));
        faces.Add(new TriangleIndices(null, new Vector3Int(0, 1, 7)));
        faces.Add(new TriangleIndices(null, new Vector3Int(0, 7, 10)));
        faces.Add(new TriangleIndices(null, new Vector3Int(0, 10, 11)));

        // 5 adjacent faces
        faces.Add(new TriangleIndices(null, new Vector3Int(1, 5, 9)));
        faces.Add(new TriangleIndices(null, new Vector3Int(5, 11, 4)));
        faces.Add(new TriangleIndices(null, new Vector3Int(11, 10, 2)));
        faces.Add(new TriangleIndices(null, new Vector3Int(10, 7, 6)));
        faces.Add(new TriangleIndices(null, new Vector3Int(7, 1, 8)));

        // 5 faces around point 3
        faces.Add(new TriangleIndices(null, new Vector3Int(3, 9, 4)));
        faces.Add(new TriangleIndices(null, new Vector3Int(3, 4, 2)));
        faces.Add(new TriangleIndices(null, new Vector3Int(3, 2, 6)));
        faces.Add(new TriangleIndices(null, new Vector3Int(3, 6, 8)));
        faces.Add(new TriangleIndices(null, new Vector3Int(3, 8, 9)));

        // 5 adjacent faces
        faces.Add(new TriangleIndices(null, new Vector3Int(4, 9, 5)));
        faces.Add(new TriangleIndices(null, new Vector3Int(2, 4, 11)));
        faces.Add(new TriangleIndices(null, new Vector3Int(6, 2, 10)));
        faces.Add(new TriangleIndices(null, new Vector3Int(8, 6, 7)));
        faces.Add(new TriangleIndices(null, new Vector3Int(9, 8, 1)));


        roots = new List<Tile>();

        // refine triangles
        for (int i = 0; i < recursion; i++)
        {
            faces2.Clear();
            foreach (var face in faces)
            {
                Tile tile = null;
                if (face.tile == null)
                {
                    tile = new Tile(this, null,
                        vertList[face.index.x],
                        vertList[face.index.y],
                        vertList[face.index.z]);

                    tile.index = roots.Count;

                    roots.Add(tile);
                }
                else
                {
                    tile = new Tile(this, face.tile,
                        vertList[face.index.x],
                        vertList[face.index.y],
                        vertList[face.index.z]);

                    tile.index = face.tile.index * 10 + face.tile.children.Count;

                    face.tile.children.Add(tile);
                }

                // replace triangle by 4 triangles
                int a = GetMiddlePoint(face.index.x, face.index.y, ref vertList, ref middlePointIndexCache, radius);
                int b = GetMiddlePoint(face.index.y, face.index.z, ref vertList, ref middlePointIndexCache, radius);
                int c = GetMiddlePoint(face.index.z, face.index.x, ref vertList, ref middlePointIndexCache, radius);

                faces2.Add(new TriangleIndices(tile, new Vector3Int(face.index.x, a, c)));
                faces2.Add(new TriangleIndices(tile, new Vector3Int(face.index.y, b, a)));
                faces2.Add(new TriangleIndices(tile, new Vector3Int(face.index.z, c, b)));
                faces2.Add(new TriangleIndices(tile, new Vector3Int(a, b, c)));
            }

            var temp = faces;
            faces = faces2;
            faces2 = temp;
        }

        Dictionary<Segment, List<Tile>> segmentToTriangeDic = new Dictionary<Segment, List<Tile>>();
        Dictionary<Vector3, List<Tile>> pointToTriangeDic = new Dictionary<Vector3, List<Tile>>();
        for (int i = 0; i < faces.Count; ++i)
        {
            var tri = faces[i];

            var tile = new Tile(this, tri.tile,
                vertList[tri.index.x],
                vertList[tri.index.y],
                vertList[tri.index.z]);

            tiles.Add(tile);
            if (tri.tile != null)
            {
                tile.index = i;
                tri.tile.children.Add(tile);
            }

            Segment ab = new Segment(tile.a, tile.b);
            Segment ac = new Segment(tile.a, tile.c);
            Segment bc = new Segment(tile.b, tile.c);

            SetNeihbors(ref segmentToTriangeDic, ab, tile);
            SetNeihbors(ref segmentToTriangeDic, ac, tile);
            SetNeihbors(ref segmentToTriangeDic, bc, tile);

            SetCorners(ref pointToTriangeDic, tile.a, tile);
            SetCorners(ref pointToTriangeDic, tile.b, tile);
            SetCorners(ref pointToTriangeDic, tile.c, tile);
        }


    }

    // return index of point in the middle of p1 and p2
    private static int GetMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        // add vertex makes sure point is on unit sphere
        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);

        // store it, return index
        cache.Add(key, i);

        return i;
    }

    private struct TriangleIndices
    {
        public Tile tile { get; private set; }
        public Vector3Int index { get; private set; }
        public TriangleIndices(Tile triangle, Vector3Int index)
        {
            this.tile = triangle;
            this.index = index;
        }
    }

    private struct Segment
    {
        public Vector3 from;
        public Vector3 to;

        public Segment(Vector3 from, Vector3 to)
        {
            this.from = from;
            this.to = to;
        }
        public override int GetHashCode()
        {
            return from.GetHashCode() + to.GetHashCode();
        }
        public static bool operator ==(Segment a, Segment b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Segment a, Segment b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Segment o = (Segment)obj;
            if ((object)o == null)
                return false;
            return (from == o.from && to == o.to) || (from == o.to && to == o.from);
        }

        public bool Equals(Segment o)
        {
            return (from == o.from && to == o.to) || (from == o.to && to == o.from);
        }
    }


    private void SetNeihbors(ref Dictionary<Segment, List<Tile>> segmentToTriangeDic, Segment segment, Tile tile)
    {
        if (segmentToTriangeDic.ContainsKey(segment))
        {
            var list = segmentToTriangeDic[segment];

            for (int i = 0; i < list.Count; ++i)
            {

                if (list[i].neihbors.Contains(tile) == false)
                {
                    list[i].neihbors.Add(tile);
                }
                if (tile.neihbors.Contains(list[i]) == false)
                {
                    tile.neihbors.Add(list[i]);
                }
            }
            list.Add(tile);
        }
        else
        {
            segmentToTriangeDic.Add(segment, new List<Tile>());
            segmentToTriangeDic[segment].Add(tile);
        }

    }

    private void SetCorners(ref Dictionary<Vector3, List<Tile>> pointToTriangeDic, Vector3 point, Tile tile)
    {
        if (pointToTriangeDic.ContainsKey(point))
        {
            var list = pointToTriangeDic[point];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].corners.ContainsKey(point) == false)
                {
                    list[i].corners.Add(point, new List<Tile>());
                }

                if (list[i].corners[point].Contains(tile) == false)
                {
                    list[i].corners[point].Add(tile);
                }

                if (tile.corners.ContainsKey(point) == false)
                {
                    tile.corners.Add(point, new List<Tile>());
                }

                if (tile.corners[point].Contains(list[i]) == false)
                {
                    tile.corners[point].Add(list[i]);
                }
            }
            list.Add(tile);
        }
        else
        {
            pointToTriangeDic.Add(point, new List<Tile>());
            pointToTriangeDic[point].Add(tile);
        }
    }

    public void Show(Material material)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].Show(material);
        }
    }

    #region FindPath

    private class PathNode
    {
        public Tile tile;
        public int h;
        public int g;
        public int f
        {
            get { return h + g; }
        }
        public Tile parent = null;

        public void Clear()
        {
            h = 0;
            g = 0;
            parent = null;
        }
    }

    private List<Tile> mOpenList = new List<Tile>();
    private List<Tile> mCloseList = new List<Tile>();
    private Dictionary<Tile, PathNode> mNodeDic = new Dictionary<Tile, PathNode>();

    private PathNode GetNode(Tile t)
    {
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

        return mNodeDic[t];
    }
    /// <summary>
    /// 网格寻路
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="isValid">返回该tile是否可走</param>
    /// <returns></returns>
    public Stack<Tile> FindPath(Tile from, Tile to, Func<Tile, bool> isValid)
    {
        Stack<Tile> result = new Stack<Tile>();

        if (from == null || to == null || isValid == null)
        {
            Debug.LogError("参数不能为空");
            return result;
        }

        mOpenList.Clear();
        mCloseList.Clear();

        var it = mNodeDic.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.Clear();
        }


        //将起点作为待处理的点放入开启列表，
        mOpenList.Add(from);

        //如果开启列表没有待处理点表示寻路失败，此路不通
        while (mOpenList.Count > 0)
        {
            //遍历开启列表，找到消费最小的点作为检查点
            Tile cur = mOpenList[0];

            var curNode = GetNode(cur);

            for (int i = 0; i < mOpenList.Count; i++)
            {
                var t = mOpenList[i];

                var node = GetNode(t);

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
                    var node = GetNode(tile);
                    if (node != null)
                    {
                        tile = node.parent;
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
            List<Tile> neighbours = cur.neihbors;
            for (int i = 0; i < neighbours.Count; i++)
            {
                var neighbour = neighbours[i];

                if (isValid(neighbour) == false || mCloseList.Contains(neighbour))
                    continue;

                int cost = curNode.g + GetCostValue(neighbour, cur);

                var neighborNode = GetNode(neighbour);

                if (cost < neighborNode.g || !mOpenList.Contains(neighbour))
                {
                    neighborNode.g = cost;
                    neighborNode.h = GetCostValue(neighbour, to);
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
    /// <summary>
    /// 获取格子权重
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected virtual int GetCostValue(Tile a, Tile b)
    {
        int cntX = (int)Mathf.Abs(a.center.x - b.center.x);
        int cntY = (int)Mathf.Abs(a.center.y - b.center.y);
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
    #endregion

    /// <summary>
    /// 根据位置定位三角形
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Tile TileAt(Vector3 worldPosition)
    {
        Ray ray = new Ray(root.position, worldPosition - root.position);

        return TileAt(ray, roots, worldPosition);
    }


    private Tile TileAt(Ray ray, List<Tile> list, Vector3 position)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var tile = list[i];
            Vector3 worldA = root.TransformPoint(tile.a);
            Vector3 worldB = root.TransformPoint(tile.b);
            Vector3 worldC = root.TransformPoint(tile.c);
            Plane plane = new Plane(worldA, worldB, worldC);

            float distance;
            if (plane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);
                if (Tile.PointInTriangle(worldA, worldB, worldC, point))
                {
                    if (tile.children.Count > 0)
                    {
                        var t = TileAt(ray, tile.children, position);
                        if (t != null)
                        {
                            return t;
                        }
                    }
                    else
                    {
                        return tile;
                    }
                }
            }
        }

        return null;
    }
    /// <summary>
    /// 根据位置和范围获取三角形
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public List<Tile> TilesInRange(Vector3 worldPosition, int r)
    {
        List<Tile> ret = new List<Tile>();

        var tile = TileAt(worldPosition);
        if (tile != null)
        {
            ret.Add(tile);
            List<Tile> neihbors = new List<Tile>();
            List<Tile> neihbors2 = new List<Tile>();
            neihbors.Add(tile);

            for (int i = 1; i <= r; ++i)
            {
                neihbors2.Clear();
                for (int j = neihbors.Count - 1; j >= 0; j--)
                {
                    for (int k = 0; k < neihbors[j].neihbors.Count; k++)
                    {
                        if (ret.Contains(neihbors[j].neihbors[k]) == false)
                        {
                            ret.Add(neihbors[j].neihbors[k]);
                            neihbors2.Add(neihbors[j].neihbors[k]);
                        }
                    }

                    if (i == r - 1)
                    {
                        var it = neihbors[j].corners.GetEnumerator();
                        while (it.MoveNext())
                        {
                            for (int k = 0; k < it.Current.Value.Count; k++)
                            {
                                if (ret.Contains(it.Current.Value[k]) == false)
                                {
                                    ret.Add(it.Current.Value[k]);
                                }
                            }
                        }
                    }
                }
                var temp = neihbors;
                neihbors = neihbors2;
                neihbors2 = temp;
            }


        }

        return ret;
    }
    /// <summary>
    /// 采样球面点
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector3 Sample(Vector3 worldPosition)
    {
        Ray ray = new Ray(root.position, worldPosition - root.position);
        worldPosition = ray.GetPoint(radius);
        return worldPosition;
    }

    public string ToXml()
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        doc.InsertBefore(dec, doc.DocumentElement);

        var grid = doc.CreateElement("Grid");
        doc.AppendChild(grid);
        for (int i = 0; i < roots.Count; i++)
        {
            grid.AppendChild(roots[i].ToXml(grid));
        }
        
        MemoryStream ms = new MemoryStream();
        XmlTextWriter xw = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
        xw.Formatting = Formatting.Indented;
        doc.Save(xw);

        ms = (MemoryStream)xw.BaseStream;
        byte[] bytes = ms.ToArray();
        string xml = System.Text.Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

        return xml;
    }

    public void FromXml(string xml)
    {
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(xml);

        var grid = doc.DocumentElement;

        for (int i = 0; i < grid.ChildNodes.Count; ++i)
        {
            var child = grid.ChildNodes[i] as XmlElement;
            if (child != null && child.Name == "Tile")
            {
                Tile tile = new Tile(this,null);
                tile.ParseXml(child);
                roots.Add(tile);
            }
        }
    }
}

