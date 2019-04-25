using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionRunPlugin : ActionPlugin
{ 
    private System.Random mRandom = new System.Random();

    public override void OnExcute(float deltaTime)
    {
        base.OnExcute(deltaTime);

        if (action.sync)
        {
            Vector3 direction =action.destination - agent.position;
            float displacement = deltaTime * agent.param.movespeed;
            if (direction.magnitude < displacement)
            {
                action.sync = false;
                agent.position = action.destination;

                if (action.doneWhenSync)
                {
                    action.Done();
                }
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

}

