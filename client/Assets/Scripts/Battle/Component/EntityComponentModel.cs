using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型组件
/// </summary>
public class EntityComponentModel :
    AssetEntity,
    IComponent,
    IStateAgent,
    IUpdate
{
    #region Inner Class
    class DelayTask : IPoolObject
    {
        private float mDelay;
        private Action mCallback;
        public void Init(float delay, Action callback)
        {
            mDelay = delay;

            mCallback = callback;

        }

        public void OnConstruct()
        {
            Clear();
        }

        public void OnDestruct()
        {
            Clear();      
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
    public BattleEntity agent { get { return this.GetRoot() as BattleEntity; } }

    private List<DelayTask> mDelayTasks = new List<DelayTask>();

    private AnimationClip mCurrentAnimationClip;
    public float animationSpeed { get; private set; }



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

    public IComponent Parent { get; set ; }
    public Dictionary<Type, IComponent> Components { get ; set ; }

    public void OnComponentInitialize()
    {
        animationSpeed = 1;
        LoadModel();
    }

    public void OnComponentDestroy()
    {
        
    }

    public void LoadModel()
    {
        if (gameObject != null)
        {
            return;
        }
        EntityParamModel param = agent.param;
        if (param != null)
        {
            LoadAsset(param.asset);
        }
    }

    public override void OnAssetLoad(Asset<GameObject> asset)
    {
        base.OnAssetLoad(asset);

        if (gameObject != null)
        {
            gameObject.name = agent.id.ToString();
            gameObject.transform.position = agent.position;
            gameObject.transform.rotation = agent.rotation;

#if UNITY_EDITOR
            ShowShapeRenderer();
#endif
        }

    }

    public override void OnDestruct()
    {
        mCurrentAnimationClip = null;
        mAnimator = null;
        mDelayTasks.Clear();
        base.OnDestruct();
    }
    public void OnUpdate(float deltaTime)
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
        if (gameObject != null)
        {
            gameObject.transform.position = agent.position;// Vector3.Lerp(gameObject.transform.position, agent.position, deltaTime * 10);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, agent.rotation, deltaTime * 10 * animationSpeed);

            gameObject.transform.localScale = agent.scale * Vector3.one;
        }

#if UNITY_EDITOR
        if (Main.Instance)
        {
            var type = Main.Instance.showType;
            for (int i = 0; i < mShapeRenderers.Count; i++)
            {
                mShapeRenderers[i].gameObject.SetActive(mShapeRenderers[i].type == type || type == ShapeType.All);
            }
        }
#endif
    }
    public void OnAgentCancel(State state)
    {

    }

    public void OnAgentEnter(State state)
    {

    }

    public void OnAgentExcute(State state, float deltaTime)
    {
        animationSpeed = state.speed;
        if (animator != null && animator.speed != animationSpeed)
        {
            animator.speed = animationSpeed;
        }

    }

    public void OnAgentExit(State state)
    {
    }

    public void OnAgentPause(State state)
    {
        if (animator != null)
        {
            animator.speed = 0;
        }
    }

    public void OnAgentResume(State state)
    {
        if (animator != null)
        {
            animator.speed = animationSpeed;
        }
    }
    public void OnAgentDestroy(State state)
    {

    }
   

    public void PlayAnimation(EntityAction action, EntityParamPluginAnimationClip clip)
    {
        if (action == null || clip == null)
        {
            return;
        }
        var animationClip = GetAnimationClip(clip.animationClip);
        if (animationClip == null)
        {
            return;
        }



        if (mCurrentAnimationClip == null
            || animationClip != mCurrentAnimationClip
            || animationClip.isLooping == false)
        {
            animator.Play("empty", 0);
            animator.Update(0);
            animator.Play(clip.animationClip, 0, clip.beginAt / animationClip.length);
            animator.speed = action.speed;
            mCurrentAnimationClip = animationClip;

           
            ShowEffect(action, clip, agent.target);
            
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
   

    private void ShowEffect(EntityAction action, EntityParamPluginAnimationClip clip,uint target)
    {
        if (action == null || clip == null || agent.param== null)
        {
            return;
        }

        var param = agent.param.GetAnimation(clip.animationClip);
        if (param == null)
        {
            return;
        }


        for (int i = 0; i < param.children.Count; ++i)
        {
            var child = param.children[i] as EntityParamEffect ;
            if (child== null || string.IsNullOrEmpty(child.asset))
            {
                continue;
            }

            float delay = child.delay - clip.beginAt;
          
            if (delay > 0)
            {
                AddDelayTask(delay, delegate ()
                {                 
                    EffectEntity effect = BattleManager.Instance.CreateEffect(child.effectType);
                    
                    if (effect != null && effect.Init(child, agent, target, null))
                    {
                        effect.action = action;
                        action.AddSubState(effect);
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

                if (effect != null && effect.Init(child, agent, target, null))
                {
                    action.AddSubState(effect);
                }
                else
                {
                    BattleManager.Instance.RemoveEffect(effect);
                }
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
        ShowRadius(ShapeType.Radius, agent.GetProperty<float>(PropertyID.PRO_RADIUS), Color.green);
        ShowRadius(ShapeType.SearchDistance, agent.GetProperty<float>(PropertyID.PRO_SEARCH_DISTANCE), Color.yellow);
        ShowRadius(ShapeType.AttackDistance, agent.GetProperty<float>(PropertyID.PRO_ATTACK_DISTANCE), Color.red);
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

