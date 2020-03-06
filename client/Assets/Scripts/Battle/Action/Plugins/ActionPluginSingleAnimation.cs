using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionPluginSingleAnimation : ActionPlugin
{
    protected EntityParamPluginAnimationClip mParamAnimationClip;
    private float mBeginTime;
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        PlayAnimation();

    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        mParamAnimationClip = null;
        mBeginTime = 0;
    }
    public override void OnStateExcute(float deltaTime)
    {
        base.OnStateExcute(deltaTime);

        ShowEffect(deltaTime);
    }

    protected virtual void PlayAnimation()
    {
        if (agent.model == null)
        {
            return;
        }

        EntityParamPluginAnimation animationPluginParam = param as EntityParamPluginAnimation;
        if (animationPluginParam == null)
        {
            return;
        }

        if (animationPluginParam.animations.Count > 0)
        {
            var animation = animationPluginParam.animations[0];

            PlayAnimation(animation);
        }
    }

    protected virtual void PlayAnimation(EntityParamPluginAnimationClip animationClip)
    {
        if (agent.model != null && agent.param != null)
        {   
            agent.model.PlayAnimation(action, animationClip);
            mParamAnimationClip = animationClip;
            mBeginTime = action.time;
        }
    }

    private void ShowEffect( float deltaTime)
    {
        if (mParamAnimationClip == null || agent.param == null)
        {
            return;
        }

        var param = agent.param.GetAnimation(mParamAnimationClip.animationClip);
        if (param == null)
        {
            return;
        }
        deltaTime = action.speed * deltaTime;

        for (int i = 0; i < param.children.Count; ++i)
        {
            var child = param.children[i] as EntityParamEffect;
            if (child == null || string.IsNullOrEmpty(child.asset))
            {
                continue;
            }
            float time = action.time - mBeginTime;

            float previousTime = time - deltaTime;

            float delay = (child.delay - mParamAnimationClip.beginAt) / (mParamAnimationClip.speed > 0 ? mParamAnimationClip.speed : 1);

            
            if (delay < deltaTime)
            {
                delay = deltaTime;
            }

            if (previousTime < delay && time >= delay)
            {
                EffectEntity effect = BattleManager.Instance.CreateEffect(child.effectType);

                if (effect != null && effect.Init(child, agent, agent.target, null))
                {
                    effect.action = action;
                    action.AddSubState(effect);
                }
                else
                {
                    BattleManager.Instance.RemoveEffect(effect);
                }
            }
        }
    }
}

