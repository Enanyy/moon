using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectEntityTime : EffectEntity
{
    private float mEffectTime;
    protected override void OnInit(IGameObject on)
    {
        base.OnInit(on);
        mEffectTime = 0;
        EntityParamEffectTime  timeParam = param as EntityParamEffectTime ;
        if (mEffectTime >= timeParam.duration)
        {
            OnEnd();
        }
    }
    protected override bool OnAssetLoad()
    {
        bool result = base.OnAssetLoad();

        if (gameObject)
        {
            var time = param as EntityParamEffectTime ;
            var bone = false;
            var on = GetOnAgent();
            var entity = on as BattleEntity;
            if (entity != null)
            {
                var model = entity.GetComponent<ModelComponent>();
                if (model != null)
                {
                    UpdateParticleSystemSpeed(model.animationSpeed);
                    if (time.bone != BonePoint.None && model.gameObject != null)
                    {
                        BoneObject point = BoneObject.GetBone(model.gameObject.transform, time.bone);
                        if (point != null)
                        {
                            if (time.bind)
                            {
                                gameObject.transform.SetParent(point.transform);
                                gameObject.transform.localScale = Vector3.one;
                                gameObject.transform.localPosition = Vector3.zero;
                                gameObject.transform.localRotation = Quaternion.identity;
                            }
                            else
                            {
                                gameObject.transform.localPosition = point.transform.position;
                                gameObject.transform.localRotation = point.transform.rotation;
                            }
                            bone = true;
                        }
                    }
                }
            }
            if (bone == false)
            {
                if (on != null)
                {
                    gameObject.transform.position = on.position;
                    gameObject.transform.rotation = on.rotation;
                }
                else
                {
                    OnEnd();
                }
            }
        }

        return result;
    }
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        var time = param as EntityParamEffectTime ;

        if (mEffectTime < time.duration)
        {
            float preEffectTime = mEffectTime;
            mEffectTime += deltaTime;
            if (preEffectTime < time.triggerAt && mEffectTime >= time.triggerAt)
            {
                OnTrigger();
            }
            if (mEffectTime > time.duration)
            {
                mEffectTime = time.duration;
                OnEnd();
            }
        }
    }
   
}

