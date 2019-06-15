using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectEntity :
    AssetEntity,
    IGameObject,
    IState<BattleEntity>
{
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
    public float scale { get; set; }

    public EffectEntity parentEffect { get; private set; }
    public BattleEntity agent { get;  set; }

    private uint mTarget;
    public BattleEntity target
    {
        get { return BattleManager.Instance.GetEntity(mTarget); }
    }


    public EntityParamEffect  param { get; private set; }

 
    public EntityAction action
    {
        get { return parent as EntityAction; }
    }

    public  IState<BattleEntity> parent
    {
        get;
        set;     
    }

    private float mEffectSpeed = 1;
    private ParticleSystem[] mParticleSystems;
    private Animator[] mAnimators;


    public virtual bool Init(EntityParamEffect  param, BattleEntity agent, uint target, EffectEntity parent)
    {
        this.param = param;
        this.agent = agent;
        this.parentEffect = parent;
        this.mTarget = target;
        this.scale = agent.scale;
        
        IGameObject go = GetOnAgent();

        if (go == null)
        {
            return false;
        }
        OnInit(go);
        LoadAsset(param.asset);

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
            case EffectOn.Parent: return parentEffect;
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

        if(parentEffect!= null)
        {
            parentEffect.Recycle();
        }
    }

    private int ShowEffect(EffectArise  arise)
    {
        int childCount = 0;
        if(param== null || agent == null)
        {
            return childCount;
        }
        var model = agent.GetComponent<EntityComponentModel>();
        if(model== null)
        {
            return childCount;
        }
        for(int i = 0; i < param.children.Count; ++i)
        {
            var child = param.children[i] as EntityParamEffect ;
            if (child == null || string.IsNullOrEmpty(child.asset) || child.arise != arise)
            {
                continue;
            }
            childCount++;
          
            if (child.delay > 0)
            {
                model.AddDelayTask(child.delay, delegate ()
                {                  
                    EffectEntity effect = BattleManager.Instance.CreateEffect(child.effectType);
                    if (effect != null && effect.Init(child, agent, mTarget, null))
                    {
                        if (action!= null) {
                            action.AddSubState(effect);
                        }
                    }
                    else
                    {
                        BattleManager.Instance.RemoveEffect(effect);
                    }
                });
            }
            else
            {
                EffectEntity effect = BattleManager.Instance.CreateEffect(child.effectType);

                if (effect != null && effect.Init(child, agent, mTarget, null))
                {
                    if (action != null)
                    {
                        action.AddSubState(effect);
                    }
                }
                else
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
        parentEffect = null;
        agent = null;
        mTarget = 0;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #region State
    public void OnEnter()
    {
       
    }

    public void OnExcute(float deltaTime)
    {
        
    }

    public void OnExit()
    {
       
    }

    public void OnCancel()
    {
       
    }

    public void OnPause()
    {
        
    }

    public void OnResume()
    {
       
    }

    public void Clear()
    {
       
    }
    #endregion
}

