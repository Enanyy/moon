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

    protected virtual void PlayAnimation(EntityParamPluginAnimationClip animationClip)
    {
        if (agent.model != null && agent.param != null)
        {   
            agent.model.PlayAnimation(action, animationClip);
        }
    }
}

