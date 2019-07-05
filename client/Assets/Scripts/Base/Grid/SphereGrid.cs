using System;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    /// <summary>
    /// 世界坐标
    /// </summary>
    public Vector3 a { get; private set; }
    /// <summary>
    /// 世界坐标
    /// </summary>
    public Vector3 b { get; private set; }
    /// <summary>
    /// 世界坐标
    /// </summary>
    public Vector3 c { get; private set; }
    /// <summary>
    /// 世界坐标
    /// </summary>
    public Vector3 center { get; private set; }

    public Vector3Int index { get; private set; }
   
    public List<Triangle> neihbors { get; private set; }

    public List<Triangle> children { get; private set; }

    public GameObject go { get; private set; }

    public Triangle(Vector3Int index, Vector3 a, Vector3 b, Vector3 c)
    {
        this.index = index;
        this.a = a;
        this.b = b;
        this.c = c;
        center = (a + b + c) / 3f;
        neihbors = new List<Triangle>();
        children = new List<Triangle>();
    }

    public void Show(Transform parent, Material material)
    {
        if (parent)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            verts.Add(a - parent.position);
            verts.Add(b - parent.position);
            verts.Add(c - parent.position);

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
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            var r = go.AddComponent<MeshRenderer>();
            var f = go.AddComponent<MeshFilter>();

            f.mesh = ms;
            r.material = material;

            //GameObject textGo = new GameObject("Text");
            //textGo.transform.SetParent(go.transform);
            //textGo.transform.position = center;

            //textGo.transform.rotation = Quaternion.LookRotation(go.transform.position - center);

            //TextMesh text = textGo.AddComponent<TextMesh>();
            //text.anchor = TextAnchor.MiddleCenter;
            //text.alignment = TextAlignment.Center;
            ////text.text = index.ToString();
            //text.color = Color.white;
            //text.fontSize = 32;
        }
    }
}
public class SphereGrid
{
    public List<Triangle> triangles = new List<Triangle>();

    public float radius;
    public int recursion;

    public Vector3 original = Vector3.zero;

    public List<Triangle> roots = new List<Triangle>();

    // return index of point in the middle of p1 and p2
    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
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
        public Triangle triangle { get; private set; }
        public Vector3Int index { get; private set; }
        public TriangleIndices(Triangle triangle, Vector3Int index)
        {
            this.triangle = triangle;
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
            return (from == o.from && to == o.to);
        }

        public bool Equals(Segment o)
        {
            return (from == o.from && to == o.to);
        }
    }

    public void GenerateTriangle(float radius, int recursion)
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


        roots = new List<Triangle>();
       
        // refine triangles
        for (int i = 0; i < recursion; i++)
        {
            faces2.Clear();
            foreach (var tri in faces)
            {
                var triangle = new Triangle(tri.index, vertList[tri.index.x], vertList[tri.index.y], vertList[tri.index.z]);

                if (tri.triangle == null)
                {
                    roots.Add(triangle);
                }
                else
                {
                   
                    tri.triangle.children.Add(triangle);
                }

                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.index.x, tri.index.y, ref vertList, ref middlePointIndexCache, radius);
                int b = getMiddlePoint(tri.index.y, tri.index.z, ref vertList, ref middlePointIndexCache, radius);
                int c = getMiddlePoint(tri.index.z, tri.index.x, ref vertList, ref middlePointIndexCache, radius);

                faces2.Add(new TriangleIndices(triangle, new Vector3Int(tri.index.x, a, c)));
                faces2.Add(new TriangleIndices(triangle, new Vector3Int(tri.index.y, b, a)));
                faces2.Add(new TriangleIndices(triangle, new Vector3Int(tri.index.z, c, b)));
                faces2.Add(new TriangleIndices(triangle, new Vector3Int(a, b, c)));
            }
            var temp = faces;
            faces = faces2;
            faces2 = temp;
        }

        Dictionary<Segment, List<Triangle>> segmentToTriangeDic = new Dictionary<Segment, List<Triangle>>();

        for(int i = 0; i <faces.Count; ++i)
        {
            var tri = faces[i];

            var triangle = new Triangle(tri.index, vertList[tri.index.x], vertList[tri.index.y], vertList[tri.index.z]);

            triangles.Add(triangle);
            if(tri.triangle!= null)
            {
                tri.triangle.children.Add(triangle);
            }

            Segment ab = new Segment(triangle.a, triangle.b);
            Segment ac = new Segment(triangle.a, triangle.c);
            Segment bc = new Segment(triangle.b, triangle.c);

            SetNeihbors(ref segmentToTriangeDic, ab, triangle);
            SetNeihbors(ref segmentToTriangeDic, ac, triangle);
            SetNeihbors(ref segmentToTriangeDic, bc, triangle);

        }

    }
    private void SetNeihbors(ref Dictionary<Segment, List<Triangle>> segmentToTriangeDic,Segment segment,Triangle triangle)
    {
        if (segmentToTriangeDic.ContainsKey(segment))
        {
            var list = segmentToTriangeDic[segment];

            for(int i = 0; i <list.Count; ++i)
            {

                if (list[i].neihbors.Contains(triangle)==false)
                {
                    list[i].neihbors.Add(triangle);
                }
                if(triangle.neihbors.Contains(list[i])==false)
                {
                    triangle.neihbors.Add(list[i]);
                }
            }
            list.Add(triangle);
        }
            
    }

    public void Show(Transform parent, Material material)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i].Show(parent,material);
        }
    }
}

