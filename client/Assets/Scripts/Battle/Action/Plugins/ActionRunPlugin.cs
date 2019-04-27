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
            var point = action.paths.First.Value;
            if (point.arrive == false)
            {
                Vector3 direction = point.destination - agent.position;

                float movespeed = agent.GetProperty(EntityProperty.PRO_MOVE_SPEED) * 0.01f;

                float displacement = deltaTime * movespeed;
                if (direction.magnitude < displacement)
                {
                    point.arrive = true;
                    agent.position = point.destination;
                    ObjectPool.ReturnInstance(action.paths.First.Value);
                    action.paths.RemoveFirst();
                 
                }
                else
                {
                    agent.position += direction.normalized * displacement;
                    agent.rotation = Quaternion.LookRotation(direction);
                }
            }
            else
            {
                ObjectPool.ReturnInstance(action.paths.First.Value);
                action.paths.RemoveFirst();
            }
        }
        else if (action.paths.Count == 1)
        {
           
            var point = action.paths.First.Value;
            
            if (point.arrive == false)
            {
                Vector3 direction = point.destination - agent.position;  
                float movespeed = agent.GetProperty(EntityProperty.PRO_MOVE_SPEED) * 0.01f;

                float displacement = deltaTime * movespeed;
                if (direction.magnitude < displacement)
                {
                    point.arrive = true;
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
                        agent.position += point.velocity * deltaTime;
                        agent.rotation = Quaternion.LookRotation(point.velocity);
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

