using System;
using System.Collections.Generic;
using UnityEngine;


public class EntityAction : State<BattleEntity>,IPoolObject
{
    public Vector3 velocity;
    public uint skillid;
    public uint target;

    public LinkedList<Vector3> paths = new LinkedList<Vector3>();

   
    public ActionParam param { get; private set; }
    public AnimationParam animation { get; private set; }

    public new ActionType type
    {
        get { return (ActionType)base.type; }
        set { base.type = (int)value; }
    }

    public bool isPool
    {
        get;set;
    }

    public EntityAction()
    {
       
    }
   

    public override void SetAgent(BattleEntity entity)
    {
        base.SetAgent(entity);
        if (agent.param == null)
        {
            return;
        }

        ActionParam action = agent.param.GetAction(type);
        if (action != null)
        {
            weight = action.weight;
            duration = duration == 0 ? action.duration : duration;
            animation = action.GetDefaultAnimation();
            if (animation == null)
            {
                var animations = action.GetAnimations();
                if (animations.Count > 0)
                {
                    animation = animations[0];
                }
            }
            var plugins = action.GetPlugins();
            for (int i = 0; i < plugins.Count; ++i)
            {
                try
                {
                    IState<BattleEntity> plugin = ObjectPool.GetInstance<ActionPlugin>(plugins[i].plugin);
                    if (plugin != null)
                    {
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
 
    public override void Clear()
    {
        base.Clear();
        param = null;
        animation = null;
        velocity = Vector3.zero;
        skillid = 0;
        target = 0;
        paths.Clear();
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
                    if (entity == null || Vector3.Distance(entity.position,agent.position)>= agent.param.attackDistance)
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
                    return velocity != Vector3.zero || paths.Count > 0;
                }
        }

        return base.IsValid();
    }

    public void OnCreate()
    {
        Clear();
    }

    public void OnReturn()
    {
        for(int i = 0; i <mSubStateList.Count; ++i)
        {
            var plugin = mSubStateList[i] as ActionPlugin;

            ObjectPool.ReturnInstance(plugin, plugin.GetType());
        }
        Clear();
    }
}
