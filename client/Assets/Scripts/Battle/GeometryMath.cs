using UnityEngine;
using System;

public struct Rectangle
{
    public Vector2 a, b, c, d;

    public Vector2 center
    {
        get
        {
            Vector2 ac = c - a;

            return a + ac.normalized * ac.magnitude / 2;
        }
    }

    public Rectangle(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }

    public bool Contains(Vector2 point)
    {
        if (GeometryMath.PointInTriangle(a, b, c, point)
            || GeometryMath.PointInTriangle(a, c, d, point)
            || GeometryMath.PointInSegment(a, c, point)
            || GeometryMath.PointInSegment(b, d, point))
        {
            return true;
        }
       

        return false;
    }

    public bool Intersect(Vector2 a, Vector2 b)
    { 
        if (GeometryMath.SegmentIntersect(this.a, this.b, a, b)
        || GeometryMath.SegmentIntersect(this.b, this.c, a, b)
        || GeometryMath.SegmentIntersect(this.c, this.d, a, b)
        || GeometryMath.SegmentIntersect(this.d, this.a, a, b))
        {
            return true;
        }
       

        return false;
    }

    public bool Intersect(Rectangle rectangle)
    {
        if (Intersect(rectangle.a,rectangle.b)
        || Intersect(rectangle.b, rectangle.c)
        || Intersect(rectangle.c, rectangle.d)
        || Intersect(rectangle.d, rectangle.a))
        {
            return true;
        }
        return false;
    }

}

public static class GeometryMath 
{
    public static bool SegmentInCirCle(Vector2 a, Vector2 b, Vector2 center, float radius)
    {
        float sqrRadius = radius * radius;
        float sqrToA = (center - a).sqrMagnitude;
        float sqrToB = (center - b).sqrMagnitude;
      
        if (sqrToA < sqrRadius || sqrToB < sqrRadius)
        {
            return true;
        }
        else
        {     
            float distance = DistanceFromPointToLine(a, b, center); 

            if (distance < radius)
            {
                return true;
            }
        }
        return false;
    }
    /// Distance of point P from the line passing through points A and B.
    public static float DistanceFromPointToLine(Vector2 a, Vector2 b, Vector2 p)
    {
        float s1 = -b.y + a.y;
        float s2 = b.x - a.x;
        return Mathf.Abs((p.x - a.x) * s1 + (p.y - a.y) * s2) / Mathf.Sqrt(s1 * s1 + s2 * s2);
    }

    /// Psuedo distance of point P from the line passing through points A and B.
    /// This is not the actual distance value, but a further point will always have a higher value than a nearer point.
    /// Faster than calculating the actual distance. Useful for sorting.
    public static float PseudoDistanceFromPointToLine(Vector2 a, Vector2 b, Vector2 p)
    {
        return Mathf.Abs((p.x - a.x) * (-b.y + a.y) + (p.y - a.y) * (b.x - a.x));
    }

    /// Determines which side point P lies of the line passing through points A and B
    /// Returns +1 if on one side of line, -1 if on the other, and 0 if it lies exactly on the line
    public static int SideOfLine(Vector2 a, Vector2 b, Vector2 p)
    {
        return Math.Sign((p.x - a.x) * (-b.y + a.y) + (p.y - a.y) * (b.x - a.x));
    }

    /// Determines whether point P is inside the triangle ABC
    public static bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {
        double s1 = c.y - a.y;
        double s2 = c.x - a.x;
        double s3 = b.y - a.y;
        double s4 = p.y - a.y;

        double w1 = (a.x * s1 + s4 * s2 - p.x * s1) / (s3 * s2 - (b.x - a.x) * s1);
        double w2 = (s4 - w1 * s3) / s1;
        return w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1;
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

        Vector3 sa = a - p; 
        Vector3 sb = b - p; 
        Vector3 sc = c - p;

        float angle1 = Mathf.Abs(Vector3.Angle(sa, sb));
        float angle2 = Mathf.Abs(Vector3.Angle(sb, sc));
        float angle3 = Mathf.Abs(Vector3.Angle(sc, sa));
        if (angle1 + angle2 < 180 || angle2 + angle3 < 180 || angle3 + angle1 < 180)
        {
            return false;
        }

        return true;
    }

    public static double determinant(double v1, double v2, double v3, double v4)  // 行列式
    {
        return (v1 * v4 - v2 * v3);
    }

    /// <summary>
    /// 线段是否相交
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool SegmentIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        const double ZERO = 1e-6;
        double d1 = determinant(b.x - a.x, c.x - d.x, b.y - a.y, c.y - d.y);
        if (d1 <= ZERO && d1 >= -ZERO)  // delta=0，表示两线段重合或平行
        {
            return false;
        }
        double d2 = determinant(c.x - a.x, c.x - d.x, c.y - a.y, c.y - d.y) / d1;
        if (d2 > 1 || d2 < 0)
        {
            return false;
        }
        double d3 = determinant(b.x - a.x, c.x - a.x, b.y - a.y, c.y - a.y) / d1;
        if (d3 > 1 || d3 < 0)
        {
            return false;
        }
        return true;
    }


    public static bool PointInSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        if ((p.x - a.x) * (b.y - a.y) == (b.x - a.x) * (p.y - a.y)  //叉乘 
                                                                          //保证Q点坐标在pi,pj之间 
            &&Math.Min(a.x, b.x) <= p.x && p.x <= Math.Max(a.x, b.x)
            && Math.Min(a.y, b.y) <= p.y && p.y <= Math.Max(a.y, b.y))
            return true;
        else
            return false;
    }

  
}