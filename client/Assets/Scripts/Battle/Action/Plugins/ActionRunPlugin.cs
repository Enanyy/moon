using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionRunPlugin : ActionPlugin
{ 
    private System.Random mRandom = new System.Random();

    public override void OnEnter()
    {
        base.OnEnter();
       
    }

    public override void OnExcute(float deltaTime)
    {
        base.OnExcute(deltaTime);

        if (action.paths.Count > 0)
        {
            Vector3 destination = action.paths.First.Value;
            Vector3 direction = destination - agent.position;
            float displacement = deltaTime * agent.param.movespeed;
            if (direction.magnitude < displacement)
            {
                action.paths.RemoveFirst();
            }
            else
            {
                action.velocity = direction.normalized * agent.param.movespeed;
            }
        }


        if (action.velocity != Vector3.zero)
        {
            agent.position += action.velocity * deltaTime;
           
        }
        else
        {
            action.Done();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        
    }

    public override void OnPause()
    {
        base.OnPause();
    }
}

