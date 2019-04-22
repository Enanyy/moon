using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI组件
/// </summary>
public class AIComponent : IComponent<BattleEntity>,IStateAgent<BattleEntity>
{
    public BattleEntity agent { get; set; }

    private BattleEntity mTarget;

    private BattleEntity mBlock;

    private float mTurnTime = 0;
    private Vector3 mTurnDirection;
   
    public void OnEnter(State<BattleEntity> state)
    {
        mTurnDirection = Vector3.zero;
        mTurnTime = 0;
    }

    public void OnExcute(State<BattleEntity> state, float delaTime)
    {
        EntityAction action = state as EntityAction;
        switch (action.type)
        {
            case ActionType.Idle:
            {
                if (mTarget == null || Vector3.Distance(mTarget.position, agent.position) >= agent.param.searchDistance)
                {
                    mTarget = BattleManager.Instance.FindClosestTarget(agent);
                }

                if (mTarget != null)
                {
                    float distance = Vector3.Distance(mTarget.position, agent.position);
                    if (distance < agent.param.attackDistance)
                    {

                        EntityAction attack = ObjectPool.GetInstance<EntityAction>();
                        attack.skillid = 1;
                        attack.target = mTarget.id;
                        agent.PlayAction(ActionType.Attack, attack);
                    }
                    else
                    {
                        Vector3 direction = (mTarget.position - agent.position).normalized;

                        EntityAction run = ObjectPool.GetInstance<EntityAction>();
                        run.velocity = direction * agent.param.movespeed;
                       
                        //mBlock = BattleManager.Instance.GetBlockEntity(agent);

                        //if (mBlock != null && mBlock.id != mTarget.id)
                        //{
                        //    Vector3 forward = BattleManager.Instance.MoveToWards(agent, mBlock);
                        //    if (forward != Vector3.zero)
                        //    {
                        //        mTurnTime = (agent.param.radius + mBlock.param.radius) / agent.param.movespeed;
                        //        mTurnDirection = forward;
                        //        run.velocity = mTurnDirection * agent.param.movespeed;
                        //    }
                        //}

                        agent.PlayAction(ActionType.Run, run);
                    }
                }

            }
            break;
            case ActionType.Run:
            {
                if (mTarget == null)
                {
                    mTurnTime = 0;
                    mTurnDirection = Vector3.zero;
                    action.Done();

                }
                else
                {
                    float distance = Vector3.Distance(mTarget.position, agent.position);
                    if (distance < agent.param.attackDistance)
                    {
                        action.velocity = Vector3.zero;
                        EntityAction attack = ObjectPool.GetInstance<EntityAction>();

                        attack.skillid = 1;
                        attack.target = mTarget.id;
                        agent.PlayAction(ActionType.Attack, attack);
                    }
                    else
                    {
                        var target = BattleManager.Instance.FindClosestTarget(agent);
                        if (target != null)
                        {
                            mTarget = target;
                        }




                        if (mTurnDirection == Vector3.zero)
                        {
                            Vector3 direction = (mTarget.position - agent.position).normalized;

                            action.velocity = direction * agent.param.movespeed;

                            //var block = BattleManager.Instance.GetBlockEntity(agent);

                            //if (block != null && block.id != mTarget.id && (mBlock == null || mBlock.id != block.id))
                            //{
                            //    mBlock = block;

                            //    Vector3 forward = BattleManager.Instance.MoveToWards(agent, mBlock);
                            //    if (forward != Vector3.zero)
                            //    {
                            //        mTurnTime = (agent.param.radius + block.param.radius) / agent.param.movespeed;
                            //        mTurnDirection = forward;
                            //    }
                            //}
                        }
                        else
                        {
                            action.velocity = mTurnDirection * agent.param.movespeed;
                        }
                    }

                    if (mTurnTime > 0)
                    {
                        mTurnTime -= delaTime;
                        if (mTurnTime < 0)
                        {
                            mTurnDirection = Vector3.zero;
                            mTurnTime = 0;
                        }
                    }
                }
            }
            break;
        }

    }

    public void OnCancel(State<BattleEntity> state)
    {

    }


    public void OnExit(State<BattleEntity> state)
    {
        
    }

    public void OnPause(State<BattleEntity> state)
    {
       
    }

    public void OnResume(State<BattleEntity> state)
    {
       
    }
    public void OnDestroy(State<BattleEntity> state)
    {

    }

    public void OnStart()
    {
       
    }

    public void OnUpdate(float deltaTime)
    {
        
    }

    public void OnDestroy()
    {

    }

   
}

