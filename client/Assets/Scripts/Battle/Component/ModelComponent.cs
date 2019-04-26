using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型组件
/// </summary>
public class ModelComponent :
    AssetEntity, 
    IComponent<BattleEntity>,
    IStateAgent<BattleEntity>
{
    #region Inner Class
    class DelayTask : IPoolObject
    {
        private float mDelay;
        private Action mCallback;

        public bool isPool { get; set; }

        public void Init(float delay, Action callback)
        {
            mDelay = delay;

            mCallback = callback;

        }

        public void OnCreate()
        {
            mDelay = 0;
            mCallback = null;
        }

        public void OnDestroy()
        {
            Clear();
        }

        public void OnReturn()
        {
            mDelay = 0;
            mCallback = null;
        }

        public bool DoTask(float deltaTime)
        {
            if (mDelay > 0)
            {
                mDelay -= deltaTime;
                if (mDelay <= 0)
                {
                    if (mCallback != null)
                    {
                        mCallback();
                    }
                }
            }
            return mDelay <= 0;
        }

        public void Clear()
        {
            mDelay = 0;
            mCallback = null;
        }
    }
    #endregion
    public BattleEntity agent { get; set; }
    private List<DelayTask> mDelayTasks = new List<DelayTask>();

    private AnimationClip mCurrentAnimationClip;
    public float animationSpeed { get; private set; }

    private float mActionTime;

    private Animator mAnimator;
    public Animator animator
    {
        get
        {
            if (mAnimator == null)
            {
                if (gameObject != null)
                {
                    mAnimator = gameObject.GetComponent<Animator>();

                    if (mAnimator == null)
                    {
                        mAnimator = gameObject.GetComponentInChildren<Animator>();
                    }
                }
            }
            return mAnimator;
        }
    }

    public void OnStart()
    {
        animationSpeed = 1;
        ModelParam param = agent.param;
        if (param!=null)
        {
            LoadAsset(param.assetID);
        }
    }

    protected override bool OnAssetLoad()
    {
        bool result= base.OnAssetLoad();
       

        if(result && agent!= null && agent.machine != null)
        {
            Play(agent.machine.current);
        }
        if(gameObject!= null)
        {
            gameObject.name = agent.id.ToString();
            gameObject.transform.position = agent.position;
            gameObject.transform.rotation = agent.rotation;

#if UNITY_EDITOR
            ShowShapeRenderer();
#endif
        }

        return result;
    }

    public override void OnDestroy()
    {
        mCurrentAnimationClip = null;
        mAnimator = null;
        mDelayTasks.Clear();
        base.OnDestroy();
    }
    public void OnUpdate(float deltaTime)
    {
        if(gameObject!= null)
        {
            gameObject.transform.position = agent.position;// Vector3.Lerp(gameObject.transform.position, agent.position, deltaTime * 10);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, agent.rotation, deltaTime * 10* animationSpeed);

            gameObject.transform.localScale = agent.scale * Vector3.one;            
        }

#if UNITY_EDITOR
        var type = Main.Instance.showType;
        for (int i = 0; i < mShapeRenderers.Count; i++)
        {
            mShapeRenderers[i].gameObject.SetActive(mShapeRenderers[i].type == type || type == ShapeType.All);
        }
#endif
    }
    public void OnCancel(State<BattleEntity> state)
    {

    }

    public void OnEnter(State<BattleEntity> state)
    {
        mActionTime = 0;
        Play(state);
        ShowEffect(state, EffectArise.ParentBegin);
    }

    public void OnExcute(State<BattleEntity> state,float deltaTime)
    {
        for (int i = mDelayTasks.Count - 1; i >= 0; --i)
        {
            if (mDelayTasks[i] == null || mDelayTasks[i].DoTask(deltaTime * animationSpeed))
            {
                if (mDelayTasks[i] != null)
                {
                    ObjectPool.ReturnInstance(mDelayTasks[i]);
                }
                mDelayTasks.RemoveAt(i);
            }
        }
        EntityAction action = state as EntityAction;
        if(action.animation!=null)
        {
            float triggerAt = action.animation.triggerAt / animationSpeed;
            if (mActionTime < triggerAt && action.time >= triggerAt)
            {
                ShowEffect(state, EffectArise.ParentTrigger);
            }
            mActionTime = action.time;
        }
    }

    public void OnExit(State<BattleEntity> state)
    {
        ShowEffect(state, EffectArise.ParentEnd);
    }

    public void OnPause(State<BattleEntity> state)
    {
        if(animator!=null)
        {
            animator.speed = 0;
        }
    }

    public void OnResume(State<BattleEntity> state)
    {
        if(animator!=null)
        {
            animator.speed = animationSpeed;
        }
    }
    public void OnDestroy(State<BattleEntity> state)
    {

    }

    private void Play(State<BattleEntity> state)
    {
        if(animator == null)
        {
            return;
        }

        EntityAction action = state as EntityAction;
        if(action == null || action.animation == null)
        {
            return;
        }

       
        var animationClip = GetAnimationClip(action.animation.animationClip);
        if(animationClip== null)
        {
            return;
        }

        if (animationClip.isLooping)
        {
            animationSpeed = 1;
        }
        else
        {
            animationSpeed = (action.animation.length / action.duration)* action.speed;
        }

        if (mCurrentAnimationClip == null
            || animationClip != mCurrentAnimationClip
            || animationClip.isLooping == false)
        {
            animator.Play("empty", 0);
            animator.Update(0);
            animator.Play(action.animation.animationClip, 0);
            animator.speed = animationSpeed;
            mCurrentAnimationClip = animationClip;
        }
    }

    public AnimationClip GetAnimationClip(string name)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            return null;
        }
        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; ++i)
        {
            if (animator.runtimeAnimatorController.animationClips[i].name == name)
            {
                return animator.runtimeAnimatorController.animationClips[i];
            }
        }

        return null;
    }

    public void AddDelayTask(float delay, Action callback)
    {
        if (callback == null)
        {
            return;
        }
        var delayTask = ObjectPool.GetInstance<DelayTask>();
        delayTask.Init(delay, callback);
        mDelayTasks.Add(delayTask);
    }
    private void ShowEffect(State<BattleEntity> state, EffectArise arise)
    {
        EntityAction action = state as EntityAction;
        if (action == null || action.animation == null)
        {
            return;
        }

        ShowEffect(action.animation, action.target, arise);
    }

    private void ShowEffect(AnimationParam param,uint target, EffectArise arise)
    {
        if (param == null)
        {
            return;
        }

        for (int i = 0; i < param.children.Count; ++i)
        {
            var child = param.children[i] as EffectParam;
            if (child== null || child.assetID == 0 || child.arise != arise)
            {
                continue;
            }

          
            if (child.delay > 0)
            {
                AddDelayTask(child.delay, delegate ()
                {
                    var entity = BattleManager.Instance.GetEntity(target);

                    EffectEntity effect = BattleManager.Instance.CreateEffect(child.effectType);

                    effect.Init(child, agent, entity, null);
                });
            }
            else
            {
                var entity = BattleManager.Instance.GetEntity(target);

                EffectEntity effect = BattleManager.Instance.CreateEffect(child.effectType);

                effect.Init(child, agent, entity, null);
            }
        }
    }

#if UNITY_EDITOR
    List<ShapeRenderer> mShapeRenderers=new List<ShapeRenderer>();

    void ShowShapeRenderer()
    {
        if (gameObject == null)
        {
            return;
        }
        ShowRadius(ShapeType.Radius, agent.GetProperty(EntityProperty.RADIUS)*0.01f, Color.green);
        ShowRadius(ShapeType.SearchDistance, agent.GetProperty(EntityProperty.SEARCH_DISTANCE)*0.01f, Color.yellow);
        ShowRadius(ShapeType.AttackDistance, agent.GetProperty(EntityProperty.ATTACK_DISTANCE)*0.01f, Color.red);
    }

    void ShowRadius(ShapeType type, float radius, Color color)
    {
        GameObject go = new GameObject(type.ToString());
        go.transform.SetParent(gameObject.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation= Quaternion.identity;
        var renderer = go.AddComponent<CircularRenderer>();
        renderer.radius = radius;
        renderer.color = color;
        renderer.type = type;
        mShapeRenderers.Add(renderer);
    }


#endif

}

