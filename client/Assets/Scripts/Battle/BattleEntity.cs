//////////////////////////////////////////////////////////////////////////
// 
// 文件：Scripts\Battle\BattleEntity.cs
// 作者：Lee
// 时间：2019/01/21
// 描述：战斗单位
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;


public class BattleEntity: 
    Components<BattleEntity>, 
    IGameObject,
    IStateAgent<BattleEntity>,
    IPoolObject
{
    public const float DEFAULT_NEIGHBOR_DIST = 4;
    public const int DEFAULT_MAX_NEIGHBOR = 5;
    public const float DEFAULT_TIME_HORIZON = 5f;
    public const float DEFAULT_TIME_HORIZONOBST = 5f;
    public const float DEFAULT_RADIUS = 1f;
    public const float DEFAULT_MAX_SPEED = 20f;

    public uint id;             //唯一ID  
    public uint campflag;       //阵营
    public string name;         //头顶名字
  
    public uint type;            //实体类型（1为英雄，2为士兵 枚举:EMObjType)
    public uint configid;        //配置id

    public uint hp;
 

    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
    public float scale { get; set; }
    public Vector3 syncPosition= Vector3.zero;


    public Vector3 bornPosition; //出生位置

    public StateMachine<BattleEntity> machine { get; private set; }


    public ModelParam param { get;private set; }

    private bool mPause;
    public bool isPause
    {
        get { return mPause; }
        set
        {
            mPause = value;
            if(machine!=null)
            {
                machine.isPause = mPause;
            }
        }
    }
    public bool active
    {
        get { return GetComponent<ModelComponent>()!= null; }
        set
        {
            if(value)
            {
                if(GetComponent<ModelComponent>() == null)
                {
                    var component = ObjectPool.GetInstance<ModelComponent>();
                    AddComponent(component);
                }
            }
            else
            {
                var component = GetComponent<ModelComponent>();
                ObjectPool.ReturnInstance(component);
                RemoveComponent<ModelComponent>();
            }
        }
    }
    public bool isDie
    {
        get { return hp <= 0; }
    }

    public uint target
    {
        get
        {
            if(machine!= null && machine.current!=null)
            {
                EntityAction action = machine.current as EntityAction;
                if(action!= null)
                {
                   return  action.target;
                    
                }
            }
            return 0;
        }
    }

    public Rectangle rectangle
    {
        get
        {
            float radius = param.radius;

            Vector3 forward = rotation * Vector3.forward;
            Vector3 right = rotation * Vector3.right;

            Vector2 f = new Vector2(forward.x, forward.z);
            Vector2 r = new Vector2(right.x, right.z);

            Vector2 a = new Vector2(position.x, position.z) - f * radius - r * radius;
            Vector2 b = a + r * 2 * radius;
            Vector2 c = b + f * 6 * radius;
            Vector2 d = c - r * 2 * radius;

            return new Rectangle(a, b, c, d);
        }
    }
   

    public bool isPool { get; set; }

    public BattleEntity()
    {
        machine = new StateMachine<BattleEntity>(this);

        Clear();
    }

    public override void Clear()
    {
        id = 0;
        campflag = 0;
        name = "";
     
        type = 0;
        configid = 0;             //表的ID

        scale = 1;

        hp = 0;

        position = Vector3.zero;
        rotation = Quaternion.identity;
        bornPosition = Vector3.zero;
        syncPosition =Vector3.zero;

        machine.Clear();

        base.Clear();
    }

    public bool Init(ModelParam param)
    {
        this.param = param;

        
        return true;
    }

    public void PlayAction(ActionType actionType, EntityAction action = null,bool first = false)
    {
        if (machine != null)
        {
            if (action == null)
            {
                action = ObjectPool.GetInstance<EntityAction>();
            }
            action.type = actionType;
            if (first)
            {
                machine.AddFirst(action);
            }
            else
            {
                machine.AddLast(action);
            }
        }
    }

    public EntityAction GetFirst(ActionType type)
    {
        return machine.GetFirst((int)type) as EntityAction;
    }
    public EntityAction GetLast(ActionType type)
    {
        return machine.GetLast((int)type) as EntityAction;
    }

    public override void OnUpdate(float deltaTime)
    {
        if (isPause)
        {
            return;
        }

        base.OnUpdate(deltaTime);

        scale = param.scale;

        if (machine != null)
        {
            if (machine.current == null)
            {
                PlayAction(ActionType.Idle);
            }
            machine.OnUpdate(deltaTime);

            SyncPosition();
        }
    }

    private void SyncPosition()
    {
        if (NeedSyncPosition())
        {
            EntityAction action = null;
            if (machine.current != null && machine.current.type == (int)ActionType.Run)
            {
                action = machine.current as EntityAction;

            }
            if (action == null)
            {
                if (machine.next != null && machine.next.type == (int)ActionType.Run)
                {
                    action = machine.next as EntityAction;
                    machine.current.speed = 5;
                }
            }
            if (action != null)
            {
                action.syncPosition = true;
            }
            else
            {
                if (machine.current != null)
                {
                    machine.current.speed = 5;
                }
                action = ObjectPool.GetInstance<EntityAction>();
                action.syncPosition = true;
                PlayAction(ActionType.Run, action, true);
            }
        }
    }

    public bool NeedSyncPosition()
    {
        return false;
    }

    public void DropBlood(int value)
    {
        if (active)
        {
                  
        }
    }

    
    public void Die()
    {
        int value = (int)hp;

        hp = 0;

        DropBlood(value);

       
    }

    
    public void OnCreate()
    {
        Clear();
    }
  
    public void OnReturn()
    {
       
    }
    
    public void OnDestroy()
    {
        
    }

    public void OnEnter(State<BattleEntity> state)
    {
        for(int i = 0; i < components.Count; ++i)
        {
            var agent = components[i] as IStateAgent<BattleEntity>;
            if(agent!=null)
            {
                agent.OnEnter(state);
            }
        }
    }

    public void OnExcute(State<BattleEntity> state, float deltaTime)
    {
        for (int i = 0; i < components.Count; ++i)
        {
            var agent = components[i] as IStateAgent<BattleEntity>;
            if (agent != null)
            {
                agent.OnExcute(state, deltaTime);
            }
        }
    }

    public void OnExit(State<BattleEntity> state)
    {
        for (int i = 0; i < components.Count; ++i)
        {
            var agent = components[i] as IStateAgent<BattleEntity>;
            if (agent != null)
            {
                agent.OnExit(state);
            }
        }
    }

    public void OnCancel(State<BattleEntity> state)
    {
        for (int i = 0; i < components.Count; ++i)
        {
            var agent = components[i] as IStateAgent<BattleEntity>;
            if (agent != null)
            {
                agent.OnCancel(state);
            }
        }
    }

    public void OnPause(State<BattleEntity> state)
    {
        for (int i = 0; i < components.Count; ++i)
        {
            var agent = components[i] as IStateAgent<BattleEntity>;
            if (agent != null)
            {
                agent.OnPause(state);
            }
        }
    }

    public void OnResume(State<BattleEntity> state)
    {
        for (int i = 0; i < components.Count; ++i)
        {
            var agent = components[i] as IStateAgent<BattleEntity>;
            if (agent != null)
            {
                agent.OnResume(state);
            }
        }
    }

    public void OnDestroy(State<BattleEntity> state)
    {
        for (int i = 0; i < components.Count; ++i)
        {
            var agent = components[i] as IStateAgent<BattleEntity>;
            if (agent != null)
            {
                agent.OnDestroy(state);
            }
        }
        EntityAction action = state as EntityAction;
        if(action!= null)
        {
            ObjectPool.ReturnInstance(action, action.GetType());
        }
    }
}
