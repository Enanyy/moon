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

    public GameObject go { get; private set; }

    public Triangle(Vector3Int index, Vector3 a, Vector3 b, Vector3 c)
    {
        this.index = index;
        this.a = a;
        this.b = b;
        this.c = c;
        center = (a + b + c) / 3f;
        neihbors = new List<Triangle>();
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
    static float phi = (1f + Mathf.Sqrt(5)) / 2f;
    static float PhiDistanceConstant = new Vector3(0, 1, phi).magnitude;


    private List<Triangle> mTriangles = new List<Triangle>();

    private float mShpereRadius;
    private int mSphereDetail;

    public Vector3 original = Vector3.zero;

    private void AddTriangle(Vector3Int index, Vector3 a, Vector3 b, Vector3 c)
    {
        mTriangles.Add(new Triangle(index, a * (mShpereRadius / a.magnitude) + original,
            b * (mShpereRadius / b.magnitude) + original,
            c * (mShpereRadius / c.magnitude) + original));
    }

    public void GenerateTriangle(float SphereRadius, int SphereDetail)
    {
        mShpereRadius = SphereRadius;
        mSphereDetail = SphereDetail;

        if (mSphereDetail < 1) mSphereDetail = 1;

        List<Vector3> PointTriplets = new List<Vector3>();

        // phi = (1 + root(5)) / 2 = 1.6180339887498948482045868343656
        float high = phi;
        float low = 1.0f;
        float zero = 0.0f;
        float[] settings = new float[3] { zero, low, high };
        // generate the main points
        // z is forward. So...I don't know.

        // 0, +-1, +- phi 



        // using detail level, produce loops for faces.

        /*
      POINTS ON UNIQUE RECTANGLES
      We take any point.
      eg. P1(h,z,l)
      We fork, raising z to high in both positive and negative.
      Low axis goes to zero.
      High axis goes to low.
      eg. P2a(l,+h,z)  P2b(1,-h, z) 
      Low goes to zero.
      eg. P3a(z,+l,h)   P3b(z,-l,h)
        */
        // we should take the y = 0 rectangle, and generate the 8 faces from it's 4 points
        Vector3[] set = new Vector3[4] { new Vector3(1 , 0, 2),
                                         new Vector3(1 , 0, -2),
                                         new Vector3(-1 , 0, -2),
                                         new Vector3(-1 , 0, 2) };


        Vector3 upperorigin = Vector3.zero, upperleft = Vector3.zero, upperright = Vector3.zero, lowerorigin = Vector3.zero, lowerleft = Vector3.zero, lowerright = Vector3.zero;

        for (int i = 0; i < 4; i++)
        {
            // do lower y triangle first.
            // this triangle points up.
            lowerorigin = new Vector3(set[i].x, set[i].y, set[i].z);

            // high to low, low to zero, zero to high [lower fork]
            lowerleft.x = Mathf.Sign(lowerorigin.x) * ((Mathf.Abs(lowerorigin.x) - 1 < 0) ? (Mathf.Sign(lowerorigin.x) * 2) : (Mathf.Abs(lowerorigin.x) - 1));
            lowerleft.y = Mathf.Sign(lowerorigin.y) * ((Mathf.Abs(lowerorigin.y) - 1 < 0) ? (-2) : (Mathf.Abs(lowerorigin.y) - 1));
            lowerleft.z = Mathf.Sign(lowerorigin.z) * ((Mathf.Abs(lowerorigin.z) - 1 < 0) ? (Mathf.Sign(lowerorigin.z) * 2) : (Mathf.Abs(lowerorigin.z) - 1));
            // eg. P1(h,z,l)

            lowerright.x = Mathf.Sign(lowerorigin.x) * ((Mathf.Abs(lowerleft.x) - 1 < 0) ? (2) : (Mathf.Abs(lowerleft.x) - 1));
            lowerright.y = Mathf.Sign(lowerleft.y) * ((Mathf.Abs(lowerleft.y) - 1 < 0) ? (-2) : (Mathf.Abs(lowerleft.y) - 1));
            lowerright.z = Mathf.Sign(lowerorigin.z) * ((Mathf.Abs(lowerleft.z) - 1 < 0) ? (Mathf.Sign(lowerorigin.z) * 2) : (Mathf.Abs(lowerleft.z) - 1));

            Convert2ToPhi(ref lowerorigin); Convert2ToPhi(ref lowerleft); Convert2ToPhi(ref lowerright);

            PointTriplets.Add(lowerorigin);
            PointTriplets.Add(lowerleft);
            PointTriplets.Add(lowerright);

            // begin upper triangles
            // this triangle points up.
            upperorigin = new Vector3(set[i].x, set[i].y, set[i].z);

            // these calculations are chained, and thus don't do well in their own functions.
            // high to low, low to zero, zero to high [upper fork]
            upperleft.x = Mathf.Sign(upperorigin.x) * ((Mathf.Abs(upperorigin.x) - 1 < 0) ? (Mathf.Sign(upperorigin.x) * 2) : (Mathf.Abs(upperorigin.x) - 1));
            upperleft.y = Mathf.Sign(upperorigin.y) * ((Mathf.Abs(upperorigin.y) - 1 < 0) ? (2) : (Mathf.Abs(upperorigin.y) - 1));
            upperleft.z = Mathf.Sign(upperorigin.z) * ((Mathf.Abs(upperorigin.z) - 1 < 0) ? (Mathf.Sign(upperorigin.z) * 2) : (Mathf.Abs(upperorigin.z) - 1));
            // eg. P1(h,z,l)

            upperright.x = Mathf.Sign(upperorigin.x) * ((Mathf.Abs(upperleft.x) - 1 < 0) ? (2) : (Mathf.Abs(upperleft.x) - 1));
            upperright.y = Mathf.Sign(upperleft.y) * ((Mathf.Abs(upperleft.y) - 1 < 0) ? (2) : (Mathf.Abs(upperleft.y) - 1));
            upperright.z = Mathf.Sign(upperorigin.z) * ((Mathf.Abs(upperleft.z) - 1 < 0) ? (Mathf.Sign(upperorigin.z) * 2) : (Mathf.Abs(upperleft.z) - 1));

            Convert2ToPhi(ref upperorigin); Convert2ToPhi(ref upperleft); Convert2ToPhi(ref upperright);

            PointTriplets.Add(upperorigin);
            PointTriplets.Add(upperright);
            PointTriplets.Add(upperleft);

        }

        /*
           END OF POINTS ON UNIQUE RECTANGLES ALGORITHM
        */

        /*
           BEGIN PAIRED POINTS ALGORITHM
        */
        set = new Vector3[12] {
                    new Vector3(1 , 0, 2), new Vector3(1 , 0, -2), new Vector3(-1 , 0, -2), new Vector3(-1 , 0, 2),
                    new Vector3(2, 1, 0), new Vector3(-2, 1, 0), new Vector3(-2, -1, 0), new Vector3(2, -1, 0),
                    new Vector3(0, 2, 1), new Vector3(0, -2, 1), new Vector3(0, -2, -1), new Vector3(0, 2, -1)
        };

        Vector3 origin = Vector3.zero, leftpoint = Vector3.zero, rightpoint = Vector3.zero;
        for (int i = 0; i < set.Length; i++)
        {
            origin = new Vector3(set[i].x, set[i].y, set[i].z);
            leftpoint = PairedPoint(origin, true);
            rightpoint = PairedPoint(origin, false);

            Convert2ToPhi(ref origin); Convert2ToPhi(ref leftpoint); Convert2ToPhi(ref rightpoint);

            PointTriplets.Add(origin);
            PointTriplets.Add(leftpoint);
            PointTriplets.Add(rightpoint);

        }


        // prepare tesselation verts for each point triplets.
        for (int i = 0; i < PointTriplets.Count / 3; i++)
        {

            origin = PointTriplets[i * 3];
            leftpoint = PointTriplets[i * 3 + 1];
            rightpoint = PointTriplets[i * 3 + 2];

            Vector3 leftstep = (leftpoint - origin) / mSphereDetail;
            Vector3 rightstep = (rightpoint - origin) / mSphereDetail;


            bool oddTriangle = false; // odd triangles are upside down.
            Vector3 myOrigin, leftfoot, rightfoot, temp;
            bool faceValue;
            for (int row = 0; row < mSphereDetail; row++)
            {

                myOrigin = origin + row * leftstep;
                leftfoot = myOrigin + leftstep;
                rightfoot = myOrigin + rightstep;


                // determine clockwise order for tri.
                // honestly, no idea how I came to this.
                faceValue = (Mathf.Sign(origin.x) * Mathf.Sign(origin.z) == ((origin.y != 0) ? (Mathf.Sign(origin.y)) : (1)));

                for (int n = 0; n < 1 + 2 * row; n++)
                {
                    //Debug.Log("triangle #" + (n + 1) + " m: " + myOrigin + " l: " + leftfoot + " r:" + rightfoot);
                    //Debug.Log(string.Format("({0},{1},{2})",myOrigin.x,myOrigin.y,myOrigin.z));
                    // Wizard shit. Seriously. No idea. It just works.
                    if (!faceValue)
                    {
                        AddTriangle(new Vector3Int(i,row,n), myOrigin,rightfoot,leftfoot);
                    }
                    else
                    {
                        AddTriangle(new Vector3Int(i, row, n), myOrigin,leftfoot,rightfoot);
                      
                    }

                    if (oddTriangle)
                    {
                        temp = myOrigin;
                        myOrigin = leftfoot;
                        leftfoot = temp;
                        rightfoot = myOrigin + rightstep;
                    }
                    else
                    {
                        temp = myOrigin;
                        myOrigin = rightfoot;
                        leftfoot = myOrigin - leftstep;
                        rightfoot = temp;
                    }

                    oddTriangle = !oddTriangle;
                }

                oddTriangle = false;
            }
        }
    }

    Vector3 PairedPoint(Vector3 origin, bool ReturnLeft)
    {
        Vector3 retvec = Vector3.zero;
        int comp = (ReturnLeft) ? (-1) : (1);
        // high to low, low to zero, zero to high [upper fork]
        retvec.x = Mathf.Sign(origin.x) * ((Mathf.Abs(origin.x) + 1 > 2) ? (0) : (Mathf.Abs(origin.x) + 1));
        if (Mathf.Abs(retvec.x) == 1) retvec.x = comp;
        retvec.y = Mathf.Sign(origin.y) * ((Mathf.Abs(origin.y) + 1 > 2) ? (0) : (Mathf.Abs(origin.y) + 1));
        if (Mathf.Abs(retvec.y) == 1) retvec.y = comp;
        retvec.z = Mathf.Sign(origin.z) * ((Mathf.Abs(origin.z) + 1 > 2) ? (0) : (Mathf.Abs(origin.z) + 1));
        if (Mathf.Abs(retvec.z) == 1) retvec.z = comp;

        return retvec;
    }


    void Convert2ToPhi(ref Vector3 source)
    {
        if (Mathf.Abs(source.x) > 1.1) source.x = Mathf.Sign(source.x) * phi;
        if (Mathf.Abs(source.y) > 1.1) source.y = Mathf.Sign(source.y) * phi;
        if (Mathf.Abs(source.z) > 1.1) source.z = Mathf.Sign(source.z) * phi;
    }

    public void CalculateNeihbors()
    {
        var it = mTriangles.GetEnumerator();
        float r = 0;
        while (it.MoveNext())
        {
            var tile = it.Current;
            if (r == 0)
            {
                float length = Vector3.Distance(tile.a, tile.b);
                r = Mathf.Sqrt(3) / 2 * length;
            }

            var neihbor = mTriangles.GetEnumerator();
            while (neihbor.MoveNext())
            {
                var t = neihbor.Current;
                if (t != tile)
                {
                    float distance = Vector3.Distance(t.center, tile.center);
                    if (distance <= r)
                    {
                        tile.neihbors.Add(t);
                    }
                }
            }

        }
    }

    public void Show(Transform parent, Material material)
    {
        for (int i = 0; i < mTriangles.Count; i++)
        {
            mTriangles[i].Show(parent,material);
        }
    }
}

