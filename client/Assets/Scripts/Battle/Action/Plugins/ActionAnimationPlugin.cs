﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionAnimationPlugin : ActionPlugin
{
    public override void OnEnter()
    {
        base.OnEnter();

        PlayAnimation();
    }

    public override void OnExcute(float deltaTime)
    {
        base.OnExcute(deltaTime);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void PlayAnimation()
    {
        if (agent.model == null)
        {
            return;
        }
       
        AnimationPluginParam animationPluginParam = param as AnimationPluginParam;
        if (animationPluginParam == null)
        {
            return;
        }

        if (animationPluginParam.animations.Count > 0)
        {
            agent.model.PlayAnimation(action, animationPluginParam.animations[0].animationClip);
        }
        
    }
}
