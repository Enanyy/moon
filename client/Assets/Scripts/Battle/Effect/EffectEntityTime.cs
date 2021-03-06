﻿using System;
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
    public override void OnAssetLoad(GameObject gameObject)
    {
        base.OnAssetLoad(gameObject);

        if (gameObject)
        {
            var time = param as EntityParamEffectTime ;
            var bone = false;
            var on = GetAgent();
            var entity = on as BattleEntity;
            if (entity != null)
            {
                if (entity.TryGetComponent(out EntityComponentModel model))
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

    }
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        var time = param as EntityParamEffectTime ;

        if (mEffectTime < time.duration)
        {
            mEffectTime += deltaTime;
            
            if (mEffectTime > time.duration)
            {
                mEffectTime = time.duration;
                OnEnd();
            }
        }
    }

    public override void OnStateExcute(float deltaTime)
    {
        base.OnStateExcute(deltaTime);
        if (action != null)
        {
            EntityParamEffectTime timeParam = param as EntityParamEffectTime;
            if (timeParam.syncAnimationSpeed)
            {
                UpdateParticleSystemSpeed(action.speed);
            }
        }
    }

}

