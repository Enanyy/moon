#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;


public enum ShapeType
{
    All,
    AttackDistance, // 攻击距离
    SearchDistance, //索敌范围
    Radius,      //模型半径
    Rectangle,
    None,
}

[RequireComponent(typeof(LineRenderer))]
public class ShapeRenderer : MonoBehaviour
{
    public ShapeType type;

    protected LineRenderer mRenderer;
    public uint id;

    public float width = 0.1f;
    public bool rendering = true;
    public Color color = Color.red;
    protected virtual void Start()
    {
        mRenderer = GetComponent<LineRenderer>();
        mRenderer.material = Resources.Load<Material>("r/material/line");
    }

    protected virtual void Update()
    {
        if (rendering)
        {
            mRenderer.startColor = color;
            mRenderer.endColor = color;
            mRenderer.startWidth = width;
            mRenderer.endWidth = width;
        }
        else
        {
            mRenderer.positionCount = 0;
        }
    }
}

public class CircularRenderer : ShapeRenderer
{

    public int pointCount = 50;

    public float radius = 10f;


    private float angle;

    private List<Vector3> points = new List<Vector3>();
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        angle = 360f / pointCount;
    }

    void CalculationPoints()
    {
        Vector3 v = transform.position + transform.forward * radius;
        points.Add(v);
        Quaternion r = transform.rotation;
        for (int i = 1; i < pointCount; i++)
        {
            Quaternion q = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y - (angle * i), r.eulerAngles.z);
            v = transform.position + (q * Vector3.forward) * radius;
            points.Add(v);
        }
    }
    void DrowPoints()
    {
        for (int i = 0; i < points.Count; i++)
        {
            mRenderer.SetPosition(i, points[i]);  //把所有点添加到positions里
        }
        if (points.Count > 0)   //这里要说明一下，因为圆是闭合的曲线，最后的终点也就是起点，
            mRenderer.SetPosition(pointCount, points[0]);
    }
    void ClearPoints()
    {
        points.Clear();  ///清除所有点
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (rendering)
        {
            mRenderer.positionCount = pointCount + 1;  ///这里是设置圆的点数，加1是因为加了一个终点（起点）
            CalculationPoints();
            DrowPoints();
        }

        ClearPoints();
    }
}

public class RectangleRenderer : ShapeRenderer
{
    public BattleEntity entity;
    protected override void Update()
    {
        base.Update();
        if (rendering && entity != null)
        {
            float y = transform.position.y;
            Rectangle rectangle = entity.rectangle;

            Vector3 a = new Vector3(rectangle.a.x, y, rectangle.a.y);
            Vector3 b = new Vector3(rectangle.b.x, y, rectangle.b.y);
            Vector3 c = new Vector3(rectangle.c.x, y, rectangle.c.y);
            Vector3 d = new Vector3(rectangle.d.x, y, rectangle.d.y);

            mRenderer.positionCount = 5;
            mRenderer.SetPosition(0, a);
            mRenderer.SetPosition(1, b);
            mRenderer.SetPosition(2, c);
            mRenderer.SetPosition(3, d);
            mRenderer.SetPosition(4, a);
        }
    }
}

#endif