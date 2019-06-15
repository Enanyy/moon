using UnityEngine;


public class ActionPluginRandomAnimation : ActionPluginSingleAnimation
{
    protected override void PlayAnimation()
    {
        if (agent.model == null || agent.param == null)
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
            int index = Random.Range(0, animationPluginParam.animations.Count);
            var animation = animationPluginParam.animations[index];
            if (action.param.duration != EntityParam.DEFAULT_DURATION)
            {
                action.duration = animation.length;
            }
            PlayAnimation(animation);
        }
    }
}