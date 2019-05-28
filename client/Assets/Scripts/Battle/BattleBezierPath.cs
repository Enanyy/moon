using PathCreation;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleBezierPath
{
 
   public static void GetPath(List<Vector3> points, bool isClosed, PathSpace space,
       ref List<Vector3> results, float step)
   {
       results.Clear();

       if (points.Count < 2)
       {
           Debug.LogError("Path requires at least 2 anchor points.");
           return;
       }


      var bezierPath = new BezierPath(points, isClosed, space);

      var vertexPath = new VertexPath(bezierPath, 0.3f, 0.01f);

       float length = vertexPath.length;
       float distance = 0;
       while (distance <= length)
       {
           Vector3 pos = vertexPath.GetPointAtDistance(distance, EndOfPathInstruction.Stop);

           results.Add(pos);
           distance += step;
       }
   }

   public static void GetPath(Vector3 from, Vector3 to, float step, float speed,float minheight,float maxheight, float gravity, ref List<Vector3> result)
    {
        result.Clear();
      

        //水平移动需要多久
        float duration = Vector3.Distance(from, to) / speed;
        if (duration > 0)
        {
            float halfTime = duration * 0.5f;
            float height = Math.Abs(gravity) * halfTime * halfTime / 2;
            //限制垂直高度
            height = Mathf.Clamp(height, minheight, maxheight);

            Vector3 center = (from + to) / 2f;
            center.y = height;

            List<Vector3> points = new List<Vector3>();
            points.Add(from);
            points.Add(center);
            points.Add(to);

            GetPath(points, false, PathSpace.xyz, ref result, step);
        }
    }
}

