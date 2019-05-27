using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ActionJumpPlugin :ActionPlugin
{
    private Vector3 mVelocity;          // 初速度向量


    private float mGravity = GRAVITY;        // 重力加速度9.8

    private PathPoint mTargetPoint;

    private const float HEIGHTLIMIT = 10;
    private const float SPEED = 20;
    private const float GRAVITY = -100;

    public override void OnEnter()
    {
        base.OnEnter();
        mGravity = GRAVITY;
    }

    public override void OnExcute(float deltaTime)
    {
        base.OnExcute(deltaTime);
       
        if (action.paths.Count > 0)
        {
            if(mTargetPoint == null)
            {
                mTargetPoint = action.paths.First.Value;


                //水平移动需要多久
                float duration = Vector3.Distance(agent.position, mTargetPoint.destination) / SPEED;
                float halfTime = duration * 0.5f;
                float height = Math.Abs(mGravity) * halfTime * halfTime / 2;
                //限制垂直高度
                if (height > HEIGHTLIMIT)
                {
                    mGravity = -Math.Abs(2 * HEIGHTLIMIT / (halfTime * halfTime));
                }

                if (duration > 0)
                {
                    // 通过一个式子计算初速度
                    mVelocity = new Vector3(
                        (mTargetPoint.destination.x - agent.position.x) / duration,
                        (mTargetPoint.destination.y - agent.position.y) / duration - 0.5f * mGravity * duration,
                        (mTargetPoint.destination.z - agent.position.z) / duration);

                }
                else
                {
                    mTargetPoint = null;
                    action.paths.RemoveFirst();
                }
            }

            if (mTargetPoint != null)
            {
                // 重力模拟
                mVelocity.y += mGravity * deltaTime;//v=gt

                // 这一帧移动位移
                Vector3 displacement = (mVelocity) * deltaTime;
                //剩余距离
               
                Vector3 direction = mTargetPoint.destination - agent.position;
                if (direction.magnitude >= displacement.magnitude)
                {
                    agent.position += displacement;
                    direction.y = 0;
                    agent.rotation = Quaternion.LookRotation(direction);
                }
                else
                {
                    agent.position = mTargetPoint.destination;
                    action.paths.RemoveFirst();
                    mTargetPoint = null;
                }
            }

        }
        else
        {
            action.Done();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        mTargetPoint = null;
    }

    public static void GetPath(Vector3 from, Vector3 to, float d, ref List<Vector3> list)
    {
        list.Clear();
        float gravity = GRAVITY;
        Vector3 velocity = Vector3.zero;
        
        //水平移动需要多久
        float duration = Vector3.Distance(from, to) / SPEED;
        float halfTime = duration * 0.5f;
        float height = Math.Abs(gravity) * halfTime * halfTime / 2;
        //限制垂直高度
        if (height > HEIGHTLIMIT)
        {
            gravity = -Math.Abs(2 * HEIGHTLIMIT / (halfTime * halfTime));
        }

        if (duration > 0)
        {
            // 通过一个式子计算初速度
            velocity = new Vector3(
                (to.x - from.x) / duration,
                (to.y - from.y) / duration - 0.5f * gravity * duration,
                (to.z - from.z) / duration);

            Vector3 position = from;
            while (true)
            {
                list.Add(position);
                // 重力模拟
                velocity.y += gravity * d; //v=gt

                // 这一帧移动位移
                Vector3 displacement = (velocity) * d;
                //剩余距离

                Vector3 direction = to - position;
                if (direction.magnitude >= displacement.magnitude)
                {
                   position += displacement;                 
                }
                else
                {
                    list.Add(to);
                    break;
                }
            }
           
        }
    }
}
