using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectParabolaEntity : EffectEntity
{
    private Vector3 mVelocity;          // 初速度向量

    private float mGravity = -9.8f;        // 重力加速度9.8

    private Vector3 mTargetPosition;

    private bool mFinish = false;

    private float mHeightLimit = 20;

    protected override void OnInit(IGameObject on)
    {
        base.OnInit(on);
        var parabola = param as EntityParamEffectParabola ;
        mGravity = -Math.Abs(parabola.gravity);
        mHeightLimit = parabola.heightLimit;
        if (target != null)
        {
            mTargetPosition = target.position;
            rotation = Quaternion.LookRotation(mTargetPosition - position);
        }
        else
        {
            rotation = entity.rotation;
            mTargetPosition = position + rotation * Vector3.forward * entity.GetProperty<float>(PropertyID.PRO_ATTACK_DISTANCE);
            mTargetPosition.y = entity.position.y;
        }

        mTargetPosition.y += parabola.heightOffset;

        float speed = parabola.speed;

        Vector3 origin = position;
        origin.y = mTargetPosition.y;
        //水平移动需要多久
        float duration = Vector3.Distance(origin, mTargetPosition) / speed;
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
                (mTargetPosition.x - position.x) / duration,
                (mTargetPosition.y - position.y) / duration - 0.5f * mGravity * duration,
                (mTargetPosition.z - position.z) / duration);
            mFinish = false;
        }
        else
        {
            mFinish = true;

            position = mTargetPosition;
            OnTrigger();
            OnEnd();
        }
    }

    public override void OnUpdate(float deltaTime)
    {

        base.OnUpdate(deltaTime);
       
        if (mFinish == false)
        {
            // 重力模拟
            mVelocity.y += mGravity * deltaTime;//v=gt

            // 这一帧移动位移
            Vector3 displacement = (mVelocity) * deltaTime;
            //剩余距离
            float distance = Vector3.Distance(mTargetPosition, position);

            if (distance >= displacement.magnitude)
            {
                position += displacement;
                rotation = Quaternion.LookRotation(mVelocity);
            }
            else
            {
                mFinish = true;

                position = mTargetPosition;
                OnTrigger();
                OnEnd();
            }
        }
    }
   
}

