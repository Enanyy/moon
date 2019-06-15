using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionPluginSingleAnimation : ActionPlugin
{
    public override void OnEnter()
    {
        base.OnEnter();
        PlayAnimation();

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

    protected virtual void PlayAnimation(EntityParamPluginAnimation.AnimationClip animation)
    {
        if (agent.model != null && agent.param != null)
        {
            var param = agent.param.GetAnimation(animation.animationClip);

            agent.model.PlayAnimation(action, param);
        }
    }
}

