using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionRunPlugin : ActionPlugin
{ 
    private System.Random mRandom = new System.Random();

    public override void OnExcute(float deltaTime)
    {
        base.OnExcute(deltaTime);

        if (action.paths.Count > 1)
        {
            var path = action.paths.First.Value;

            Vector3 direction = path.destination - agent.position;
            float movespeed = agent.movespeed;
            if (path.velocity != Vector3.zero)
            {
                movespeed = path.velocity.magnitude;
            }
            float displacement = deltaTime * movespeed;
            if (direction.magnitude < displacement)
            {
                path.arrive = true;
                agent.position = path.destination;

                action.paths.RemoveFirst();
            }
            else
            {
                agent.position += direction.normalized * displacement;
                agent.rotation = Quaternion.LookRotation(direction);
            }
        }
        else if (action.paths.Count == 1)
        {
            var path = action.paths.First.Value;
            if (path.arrive == false)
            {
                Vector3 direction = path.destination - agent.position;
                float movespeed = agent.movespeed;
                if (path.velocity != Vector3.zero)
                {
                    movespeed = path.velocity.magnitude;
                }
                float displacement = deltaTime * movespeed;
                if (direction.magnitude < displacement)
                {
                    path.arrive = true;
                    agent.position = path.destination;
                }
                else
                {
                    agent.position += direction.normalized * displacement;
                    agent.rotation = Quaternion.LookRotation(direction);
                }
            }

            if (path.arrive)
            {
                if (path.done)
                {
                    action.paths.RemoveFirst();
                    action.Done();
                }
                else
                {
                    if (path.velocity != Vector3.zero)
                    {
                        agent.position += path.velocity * deltaTime;
                        agent.rotation = Quaternion.LookRotation(path.velocity);
                    }
                    else
                    {
                        action.Done();
                    }
                }
            }
        }
        else
        {
            action.Done();
        }
    }

}

