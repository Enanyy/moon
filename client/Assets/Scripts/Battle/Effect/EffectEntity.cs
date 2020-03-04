using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectEntity :

    IGameObject,
    IState,
    IUpdate,
    IPoolObject
{
    public AssetEntity asset { get; private set; }
    private Vector3 mPosition;
    public Vector3 position 
    { 
        get { return mPosition; }
        set
        { 
            mPosition = value;
            if(asset!= null&& asset.gameObject)
            {
                asset.gameObject.transform.position = mPosition;
            }
        }
    }
    private Quaternion mRotation;
    public Quaternion rotation 
    {
        get { return mRotation; }
        set
        {
            mRotation = value;
            if (asset != null && asset.gameObject)
            {
                asset.gameObject.transform.rotation = mRotation;
            }
        }
    }
    private float mScale;
    public float scale {
        get { return mScale; }
        set
        {
            mScale = value;
            if (asset != null && asset.gameObject)
            {
                asset.gameObject.transform.localScale = Vector3.one * mScale;
            }
        }
    }

    public BattleEntity entity { get;  set; }

    private uint mTarget;
    public BattleEntity target
    {
        get { return BattleManager.Instance.GetEntity(mTarget); }
    }

    public EntityParamEffect param { get; private set; }

    public EntityAction action { get; set; }

    public EffectEntity parent { get; set; }

    private float mEffectSpeed = 1;
    private ParticleSystem[] mParticleSystems;
    private Animator[] mAnimators;


    public virtual bool Init(EntityParamEffect  param, BattleEntity entity, uint target, EffectEntity parent)
    {
        this.param = param;
        this.entity = entity;
        this.parent = parent;
        this.mTarget = target;
        this.scale = entity.scale;
        
        IGameObject go = GetAgent();

        if (go == null)
        {
            return false;
        }
        OnInit(go);

        if(asset == null)
        {
            asset = new AssetEntity();
        }
        asset.LoadAsset(param.asset, OnAssetLoad);

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

    protected IGameObject GetAgent()
    {
        switch (param.on)
        {
            case EffectOn.Self: return entity;
            case EffectOn.Parent: return parent;
            case EffectOn.Target: return target;
        }
        return null;
    }

    public virtual void OnAssetLoad(GameObject gameObject)
    {
       
        if (gameObject != null)
        {
            gameObject.transform.localScale = entity.scale * Vector3.one;
            mParticleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
            mAnimators = gameObject.GetComponentsInChildren<Animator>();
        }
    }
    protected void UpdateParticleSystemSpeed(float speed)
    {
        if(asset== null)
        {
            return;
        }
        if (asset.gameObject == null || asset.gameObject.activeSelf == false)
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

    public virtual void Recycle()
    {
        BattleManager.Instance.RemoveEffect(this);

        if(asset!=null)
        {
            asset.Recycle();
        }

        if(parent!= null)
        {
            parent.Recycle();
        }
    }

    private int ShowEffect(EffectArise  arise)
    {
        int childCount = 0;
        if(param== null || entity == null)
        {
            return childCount;
        }
        if(entity.TryGetComponent(out EntityComponentModel model)==false)
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
                    if (effect != null && effect.Init(child, entity, mTarget, null))
                    {
                        if (action!= null) {
                            effect.action = action;
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

                if (effect != null && effect.Init(child, entity, mTarget, null))
                {
                    if (action != null)
                    {
                        effect.action = action;
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

    public virtual void OnConstruct()
    {
        
    }

    public virtual void OnDestruct()
    {
        asset = null;
        mAnimators = null;
        mParticleSystems = null;
        mEffectSpeed = 1;
        parent = null;
        entity = null;
        mTarget = 0;
    }
    #region State
    public virtual void OnStateEnter()
    {
       
    }

    public virtual void OnStateExcute(float deltaTime)
    {
       
    }

    public virtual void OnStateExit()
    {
       
    }

    public virtual void OnStateCancel()
    {
       
    }

    public virtual void OnStatePause()
    {
        
    }

    public virtual void OnStateResume()
    {
       
    }

    public virtual void Clear()
    {
       
    }

    public virtual void OnStateDestroy()
    {

    }
    #endregion
}

