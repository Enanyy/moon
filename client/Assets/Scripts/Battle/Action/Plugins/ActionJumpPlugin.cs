using UnityEngine;
using System.Collections;
using System;

public class ActionJumpPlugin :ActionPlugin
{
    private Vector3 mVelocity;          // 初速度向量

    private float mGravity = -50f;        // 重力加速度9.8

    private PathPoint mTargetPoint;

    private float mHeightLimit = 20;
    private float mSpeed = 20;

    public override void OnEnter()
    {
        base.OnEnter();
        mGravity = -100f;
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
                float duration = Vector3.Distance(agent.position, mTargetPoint.destination) / mSpeed;
                float halfTime = duration * 0.5f;
                float height = Math.Abs(mGravity) * halfTime * halfTime / 2;
                //限制垂直高度
                if (height > mHeightLimit)
                {
                    mGravity = -Math.Abs(2 * mHeightLimit / (halfTime * halfTime));
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
                float distance = Vector3.Distance(mTargetPoint.destination, agent.position);

                if (distance >= displacement.magnitude)
                {
                    agent.position += displacement;
                   
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
}
