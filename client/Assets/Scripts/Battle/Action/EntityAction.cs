using System;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint:IPoolObject
{
    public Vector3 destination;
    public Vector3 velocity;
    public bool done ;
    public bool arrive;

    public bool isPool { get; set; }

    public PathPoint()
    {

    }

    public void Init(Vector3 destination, Vector3 velocity, bool done)
    {
        this.destination = destination;
        this.velocity = velocity;
        this.done = done;
        this.arrive = false;
    }

    public void OnCreate()
    {

    }
    public void OnReturn()
    {
        Init(Vector3.zero, Vector3.zero, false);
    }
    public void OnDestroy()
    {

    }
}

public class EntityAction : State<BattleEntity>,IPoolObject
{
    
    public uint skillid;
    public uint target;
    public LinkedList<PathPoint> paths { get; private set; }



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
       paths = new LinkedList<PathPoint>();
    }
    public void AddPathPoint(Vector3 destination,Vector3 velocity, bool done)
    {
        var point = ObjectPool.GetInstance<PathPoint>();
        point.Init(destination, velocity, done);
        paths.AddLast(point);

       
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
       
        skillid = 0;
        target = 0;
        if (paths != null)
        {
            paths.Clear();
        }
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
