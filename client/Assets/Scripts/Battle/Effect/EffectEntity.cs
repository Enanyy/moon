﻿using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectEntity :AssetEntity,IGameObject
{
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
    public float scale { get; set; }

    public EffectEntity parent { get; private set; }
    public BattleEntity agent { get; private set; }

    private uint mTarget;
    public BattleEntity target
    {
        get { return BattleManager.Instance.GetEntity(mTarget); }
    }


    public EffectParam param { get; private set; }


    private float mEffectSpeed = 1;
    private ParticleSystem[] mParticleSystems;
    private Animator[] mAnimators;


    public virtual bool Init(EffectParam param, BattleEntity agent, uint target, EffectEntity parent)
    {
        this.param = param;
        this.agent = agent;
        this.parent = parent;
        this.mTarget = target;
        this.scale = agent.scale;
        
        IGameObject go = GetOnAgent();

        if (go == null)
        {
            return false;
        }
        OnInit(go);
        LoadAsset(param.assetID);

        OnBegin();
        return true;
    }

    protected virtual void OnInit(IGameObject on)
    {
        if (on != null)
        {
            Vector3 pos = on.position;
            pos += on.rotation * Vector3.right * param.offset.x;
            pos += on.rotation * Vector3.forward * param.offset.z;
            pos += on.rotation * Vector3.up * param.offset.y;
            position = pos;
            rotation = on.rotation;
            scale = on.scale;
        }
    }

    protected IGameObject GetOnAgent()
    {
        switch (param.on)
        {
            case EffectOn.Self: return agent;
            case EffectOn.Parent: return parent;
            case EffectOn.Target: return target;
        }
        return null;
    }

    protected override bool OnAssetLoad()
    {
        bool result = base.OnAssetLoad();
        if (gameObject != null)
        {
            gameObject.transform.localScale = agent.scale * Vector3.one;
            mParticleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
            mAnimators = gameObject.GetComponentsInChildren<Animator>();
        }

        return result;
    }
    protected void UpdateParticleSystemSpeed(float speed)
    {
        if (gameObject == null || gameObject.activeSelf == false)
        {
            return;
        }

        if (mEffectSpeed == speed)
        {
            return;
        }
        mEffectSpeed = speed;

        if (mParticleSystems != null)
        {
            for (int i = 0; i < mParticleSystems.Length; ++i)
            {
                var main = mParticleSystems[i].main;
                main.simulationSpeed = mEffectSpeed;
            }
        }
        if (mAnimators != null)
        {
            for (int i = 0; i < mAnimators.Length; ++i)
            {
                mAnimators[i].speed = mEffectSpeed;
            }
        }

    }

    public virtual void OnUpdate(float deltaTime)
    {

    }

    protected virtual void OnBegin()
    {
        ShowEffect(EffectArise.ParentBegin);
    }

    protected virtual void OnTrigger()
    {
        ShowEffect(EffectArise.ParentTrigger);
    }

    protected virtual void OnEnd()
    {
        int childCount = ShowEffect(EffectArise.ParentEnd);
        if(childCount == 0)
        {
            Recycle();
        }
    }

    public override void Recycle()
    {
        BattleManager.Instance.RemoveEffect(this);

        base.Recycle();

        if(parent!= null)
        {
            parent.Recycle();
        }
    }

    private int ShowEffect(EffectArise  arise)
    {
        int childCount = 0;
        if(param== null || agent == null)
        {
            return childCount;
        }
        var model = agent.GetComponent<ModelComponent>();
        if(model== null)
        {
            return childCount;
        }
        for(int i = 0; i < param.children.Count; ++i)
        {
            var child = param.children[i] as EffectParam;
            if (child == null || child.assetID == 0 || child.arise != arise)
            {
                continue;
            }
            childCount++;
          
            if (child.delay > 0)
            {
                model.AddDelayTask(child.delay, delegate ()
                {                  
                    EffectEntity effect = BattleManager.Instance.CreateEffect(child.effectType);
                    if (effect.Init(child, agent, mTarget, this) == false)
                    {
                        BattleManager.Instance.RemoveEffect(effect);
                    }
                });
            }
            else
            {
                EffectEntity effect = BattleManager.Instance.CreateEffect(child.effectType);

                if (effect.Init(child, agent, mTarget, this) == false)
                {
                    BattleManager.Instance.RemoveEffect(effect);
                }
            }
        }
        return childCount;
    }

    public override void OnCreate()
    {
        base.OnCreate();
    }

    public override void OnReturn()
    {
        base.OnReturn();
        mAnimators = null;
        mParticleSystems = null;
        mEffectSpeed = 1;
        parent = null;
        agent = null;
        mTarget = 0;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

   
}

