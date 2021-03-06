﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using PathCreation;

public class ActionJumpPlugin :ActionPlugin
{
    private PathPoint mTargetPoint;

    public const float MAXHEIGHT = 15;
    public const float MINHEIGHT = 5;
    public const float SPEED = 100;
    public const float GRAVITY = -1000;


    private BezierPath mBezierPath;
    private VertexPath mVertexPath;

    private float mDistance = 0;
  
    public override void OnStateEnter()
    {
        base.OnStateEnter();

        mTargetPoint = null;
        mDistance = 0;
    }

    public override void OnStateExcute(float deltaTime)
    {
        base.OnStateExcute(deltaTime);
       
        if (action.paths.Count > 0)
        {
            if (mTargetPoint == null)
            {
                mTargetPoint = action.paths.First.Value;
                mDistance = 0;

                if (Chess.Instance.grid == BattleGrid.Rect)
                {
                    var tile = BattleRectGrid.Instance.TileAt(mTargetPoint.destination);
                    if (tile == null || tile.isValid == false)
                    {
                        action.Done();

                        var point = action.paths.First;
                        while (point!= null)
                        {
                            point.Value.Failed(agent);
                            ObjectPool.ReturnInstance(point.Value);
                            point = point.Next;

                        }
                        action.paths.Clear();
                        mTargetPoint = null;

                    }
                }
                else
                {
                    var tile = BattleHexGrid.Instance.TileAt(mTargetPoint.destination);
                    if (tile == null || tile.isValid == false)
                    {
                        action.Done();
                        var point = action.paths.First;
                        while (point != null)
                        {
                            point.Value.Failed(agent);
                            ObjectPool.ReturnInstance(point.Value);
                            point = point.Next;

                        }
                        action.paths.Clear();
                        mTargetPoint = null;

                    }

                }
                if (mTargetPoint != null)
                {
                    //水平移动需要多久
                    float duration = Vector3.Distance(agent.position, mTargetPoint.destination) / SPEED;
                    if (duration > 0)
                    {
                        float halfTime = duration * 0.5f;
                        float height = Math.Abs(GRAVITY) * halfTime * halfTime / 2;
                        //限制垂直高度
                        height = Mathf.Clamp(height, MINHEIGHT, MAXHEIGHT);

                        Vector3 center = (agent.position + mTargetPoint.destination) / 2f;
                        center.y = height;

                        List<Vector3> points = new List<Vector3>();
                        points.Add(agent.position);
                        points.Add(center);
                        points.Add(mTargetPoint.destination);

                        mBezierPath = new BezierPath(points, false, PathSpace.xyz);
                        mVertexPath = new VertexPath(mBezierPath, 0.3f, 0.01f);

                    }
                    else
                    {
                        mTargetPoint.Arrive(agent);
                        ObjectPool.ReturnInstance(mTargetPoint);
                        mTargetPoint = null;
                        action.paths.RemoveFirst();
                    }
                }
            }

            if (mTargetPoint != null)
            {
                Vector3 direction = mTargetPoint.destination - agent.position;

                agent.position = mVertexPath.GetPointAtDistance(mDistance, EndOfPathInstruction.Stop);

                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    agent.rotation = Quaternion.LookRotation(direction);
                }

                mDistance += SPEED * deltaTime;
                if (mDistance > mVertexPath.length)
                {
                    agent.position = mTargetPoint.destination;

                    mDistance = 0;
                    mTargetPoint.Arrive(agent);
                    ObjectPool.ReturnInstance(mTargetPoint);

                    mTargetPoint = null;
                    action.paths.RemoveFirst();
                }
            }

        }
        else
        {
            action.Done();
        }
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        mTargetPoint = null;
    }
}
