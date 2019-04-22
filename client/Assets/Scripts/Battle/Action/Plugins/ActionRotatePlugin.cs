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
        else
        {
            if (action.velocity != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(action.velocity);
                agent.rotation = Quaternion.Lerp(agent.rotation,rotation, deltaTime * 10);
            }
        }
    }
}

