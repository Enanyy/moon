using System;
using UnityEngine;

public class ActionRotatePlugin : ActionPlugin
{
    private BattleEntity mTargetEntity;
    public override void OnExcute(float deltaTime)
    {
        base.OnExcute(deltaTime);

        if (action.type == ActionType.Attack)
        {
            if (mTargetEntity == null || mTargetEntity.id != action.target)
            {
                mTargetEntity = BattleManager.Instance.GetEntity(action.target);
            }

            if (mTargetEntity != null)
            {
                var direction = mTargetEntity.position - agent.position;
                agent.rotation = Quaternion.Lerp(agent.rotation, Quaternion.LookRotation(direction), deltaTime * 10);
            }
        }
        else if(action.type== ActionType.Run)
        {
            if (action.paths.Count > 0)
            {
                var point = action.paths.First.Value;
                if (point.arrive == false)
                {
                    agent.rotation = Quaternion.LookRotation(point.destination - agent.position);
                }
                else
                {
                    if (point.velocity != Vector3.zero)
                    {
                        agent.rotation = Quaternion.LookRotation(point.velocity);
                    }
                }
            }
        }
        
    }
}

