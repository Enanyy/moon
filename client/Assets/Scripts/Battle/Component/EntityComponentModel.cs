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
    
    public BattleEntity agent { get { return this.GetRoot() as BattleEntity; } }


    private AnimationClip mCurrentAnimationClip;
    public float animationSpeed { get; private set; }

    private EntityParamPluginAnimationClip mParamAnimationClip;


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
   
        base.OnDestruct();
    }

   

    public void OnUpdate(float deltaTime)
    {
        
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
        EntityAction action = state as EntityAction;
        if (action.type == ActionType.Run)
        {
            if (agent.param != null)
            {
                animationSpeed = agent.GetProperty<float>(PropertyID.PRO_MOVE_SPEED, 0) / agent.param.defaultSpeed;
            }
            else
            {
                animationSpeed = 1;
            }
        }
        else
        {
            animationSpeed = state.speed * (mParamAnimationClip != null ? mParamAnimationClip.speed : 1);
        }
        if (animator != null && animator.speed != animationSpeed)
        {
            animator.speed = animationSpeed;
        }
    }

    public void OnAgentExit(State state)
    {
        mParamAnimationClip = null;
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
            if (clip.beginAt > 0)
            {
                animator.Play(clip.animationClip, 0, clip.beginAt / animationClip.length);
            }else
            {
                animator.CrossFade(clip.animationClip, 0.5f, 0);
            }
          
            mCurrentAnimationClip = animationClip;

            mParamAnimationClip = clip;
         
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

   



#if UNITY_EDITOR
    List<ShapeRenderer> mShapeRenderers=new List<ShapeRenderer>();

    void ShowShapeRenderer()
    {
        if (gameObject == null)
        {
            return;
        }
        ShowRadius(ShapeType.Radius, agent.GetProperty<float>(PropertyID.PRO_RADIUS,0), Color.green);
        ShowRadius(ShapeType.SearchDistance, agent.GetProperty<float>(PropertyID.PRO_SEARCH_DISTANCE, 0), Color.yellow);
        ShowRadius(ShapeType.AttackDistance, agent.GetProperty<float>(PropertyID.PRO_ATTACK_DISTANCE, 0), Color.red);
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

