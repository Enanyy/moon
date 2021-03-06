﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint:IPoolObject
{
    public Vector3 destination;
    public Vector3 velocity;
    public bool done ;
    public bool arrive;

    private Action<BattleEntity, Vector3> mArriveAction;
    private Action<BattleEntity, Vector3> mFailedAction;

    public PathPoint()
    {

    }

    public void Init(Vector3 destination, Vector3 velocity, bool done,Action<BattleEntity, Vector3> arriveAction = null, Action<BattleEntity, Vector3> failedAction= null)
    {
        this.destination = destination;
        this.velocity = velocity;
        this.done = done;
        this.arrive = false;
        this.mArriveAction = arriveAction;
        this.mFailedAction = failedAction;
    }

    public void Arrive(BattleEntity entity)
    {
        if (mArriveAction != null)
        {
            mArriveAction(entity, destination);
        }
    }

    public void Failed(BattleEntity entity)
    {
        if (mFailedAction != null)
        {
            mFailedAction(entity, destination);
        }
    }

    public void OnConstruct()
    {
        
    }
    public void OnDestruct()
    {
        
    }
}

public class EntityAction : State,IPoolObject
{
    public BattleEntity agent { get; private set; }
    
    public uint skillid;
    public uint target;
    public LinkedList<PathPoint> paths { get; private set; }

    public EntityParamAction param { get; private set; }
   
    public new ActionType type
    {
        get { return (ActionType)base.type; }
        set { base.type = (int)value; }
    }
    public EntityAction()
    {
       paths = new LinkedList<PathPoint>();
    }
    
    public void AddPathPoint(Vector3 destination,Vector3 velocity, bool done, Action<BattleEntity, Vector3> arriveAction= null, Action<BattleEntity, Vector3> failedAction = null)
    {
        var point = ObjectPool.GetInstance<PathPoint>();
        point.Init(destination, velocity, done, arriveAction, failedAction);
        paths.AddLast(point);    
    }

    public void ClearPath()
    {
        if (paths != null)
        {
            var it = paths.GetEnumerator();
            while (it.MoveNext())
            {
                ObjectPool.ReturnInstance(it.Current);
            }
            paths.Clear();
        }
    }

    public virtual void SetAgent(BattleEntity entity)
    {
        agent = entity;
        if (agent== null || agent.param == null)
        {
            return;
        }

        param = agent.param.GetAction(type);
        if (param != null)
        {
            weight = param.weight;
            duration = duration == 0 ? param.duration : duration;

            var plugins = param.GetParams<EntityParamPlugin>();
            for (int i = 0; i < plugins.Count; ++i)
            {
                try
                {
                    ActionPlugin plugin = ObjectPool.GetInstance<ActionPlugin>(plugins[i].plugin);
                    if (plugin != null)
                    {
                        plugin.Init(plugins[i]);
                        plugin.agent = agent;
                        plugin.action = this;
                        AddSubState(plugin);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);

                }
            }
        }     
    }
    public override void OnStateCancel()
    {
        base.OnStateCancel();
        if (agent != null)
        {
            agent.OnAgentCancel(this);
        }
    }
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if (agent != null)
        {
            agent.OnAgentEnter(this);
        }
    }
    public override void OnStateExcute(float deltaTime)
    {
        base.OnStateExcute(deltaTime);
        if (agent != null)
        {
            agent.OnAgentExcute(this, deltaTime);
        }
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        if(agent!= null)
        {
            agent.OnAgentExit(this);
        }
    }
    public override void OnStatePause()
    {
        base.OnStatePause();
        if(agent!=null)
        {
            agent.OnAgentPause(this);
        }
    }
    public override void OnStateResume()
    {
        base.OnStateResume();
        if(agent!=null)
        {
            agent.OnAgentResume(this);
        }
    }
    public override void OnStateDestroy()
    {
        base.OnStateDestroy();
        if(agent!= null)
        {
            agent.OnAgentDestroy(this);
        }
    }

    public override void Clear()
    {
        base.Clear();
        param = null;

       
        skillid = 0;
        target = 0;
        ClearPath();
    }

    public override bool IsValid()
    {
        switch (type)
        {
            case ActionType.Attack:
                {             
                    if(agent.isDie)
                    {
                        return false;
                    }
                    var entity = BattleManager.Instance.GetEntity(target);
                    if (entity == null || entity.isDie)
                    {
                        return false;
                    }

                }break;
            case ActionType.Run:
                {
                    if (agent.isDie)
                    {
                        return false;
                    }
                    return paths.Count > 0;
                }
        }

        return base.IsValid();
    }
    public override bool IsCancel()
    {
        if (param != null)
        {
            if (param.afterAt > 0 && time >= param.afterAt)
            {
                return false;
            }
        }
        return base.IsCancel();
    }

    public override bool IsSkipping()
    {
        if (param != null && machine.next != null && machine.next.type != (int)type)
        {
            if ((param.beforeAt > 0 && time < param.beforeAt) || (param.afterAt > 0 && time >= param.afterAt))
            {
                return true;
            }
        }

        return base.IsSkipping();
    }

    public void OnConstruct()
    {
        Clear();
    }
    public void OnDestruct()
    {
        for (int i = 0; i < mSubStateList.Count; ++i)
        {
            var plugin = mSubStateList[i] as ActionPlugin;

            if (plugin != null)
            {
                plugin.agent = null;
                plugin.action = null;

                ObjectPool.ReturnInstance(plugin, plugin.GetType());
            }
        }
        Clear();
    }
}
