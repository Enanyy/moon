﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionPluginRun : ActionPlugin
{  
    public override void OnStateExcute(float deltaTime)
    {
        base.OnStateExcute(deltaTime);

        if (action.paths.Count > 0)
        {           
            var point = action.paths.First.Value;
            
            if (point.arrive == false)
            {
                Vector3 direction = point.destination - agent.position;  
                float movespeed = agent.properties.GetProperty<float>((uint)PropertyID.PRO_MOVE_SPEED,0);

                float displacement = deltaTime * movespeed;
                if (direction.magnitude < displacement)
                {
                    point.arrive = true;
                    point.Arrive(agent);
                    agent.position = point.destination;
                }
                else
                {
                    agent.position += direction.normalized * displacement;
                    agent.rotation = Quaternion.LookRotation(direction);
                }               
            }

            if (point.arrive)
            {
                if (action.paths.Count > 1)
                {
                    ObjectPool.ReturnInstance(action.paths.First.Value);
                    action.paths.RemoveFirst();
                }
                else
                {
                    if (point.done)
                    {
                        ObjectPool.ReturnInstance(action.paths.First.Value);
                        action.paths.RemoveFirst();
                        action.Done();
                    }
                    else
                    {
                        if (point.velocity != Vector3.zero)
                        {
                            agent.position += point.velocity * deltaTime * 0.9f;
                            agent.rotation = Quaternion.LookRotation(point.velocity);
                        }
                        else
                        {
                            action.Done();
                        }
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

