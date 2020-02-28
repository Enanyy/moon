using UnityEngine;

public class ActionPluginMultitudeAnimation : ActionPluginSingleAnimation
{
    private int mIndex = -1;
    private float mTime;
    protected override void PlayAnimation()
    {
        Play(0);
    }

    public override void OnStateExcute(float deltaTime)
    {
        base.OnStateExcute(deltaTime);

        if(mIndex >= 0)
        {
            if(action.time >= mTime)
            {
                Play(mIndex + 1);
            }
        }
    }

    private void Play(int index)
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

        if (animationPluginParam.animations.Count > index)
        {
            var animation = animationPluginParam.animations[index];
            mTime += animation.length;
            mIndex = index;
            PlayAnimation(animation);
        }
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        mIndex = -1;
        mTime = 0;
    }
}