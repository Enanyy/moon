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
public enum PropertyID
{
    PRO_HP = 1,
    PRO_MAX_HP = 2,
    PRO_ATTACK = 3,
    PRO_DEFENSE = 4,
    PRO_MOVE_SPEED = 5,
    PRO_ATTACK_DURATION = 6,
    PRO_SEARCH_DISTANCE = 7,
    PRO_ATTACK_DISTANCE = 8,
    PRO_RADIUS = 9,
}

public class BattleEntity: 
    IComponent,
    IGameObject,
    IStateAgent,
    IPoolObject,
    IUpdate
{

    public uint id;             //唯一ID  
    public uint campflag;       //阵营
    public string name;         //头顶名字
  
    public uint type;            //实体类型（1为英雄，2为士兵 枚举:EMObjType)
    public string config;        //配置id

  

    public virtual Vector3 position { get; set; }
    public virtual Quaternion rotation { get; set; }
    public float scale { get; set; }

    public Properties properties = new Properties();

    public StateMachine machine { get; private set; }


    public EntityParamModel param { get;private set; }

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
    public  EntityComponentModel model { get; private set; }
    public bool active
    {
        get { return model!= null; }
        set
        {
            if(value)
            {
                if(model == null)
                {
                    model = ObjectPool.GetInstance<EntityComponentModel>();
                    this.AddComponent(model);
                }
            }
            else
            {
                if (model != null)
                {
                    ObjectPool.ReturnInstance(model);
                    this.RemoveComponent(model);
                    model = null;
                }
            }
        }
    }
    public bool isDie{get; private set;}

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
            float radius = properties.GetProperty<float>((uint)PropertyID.PRO_RADIUS, 0);

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

    public IComponent Parent { get ; set; }
    public Dictionary<Type, IComponent> Components { get; set ; }

    public BattleEntity()
    {
        machine = new StateMachine();
        Clear();
    }

    public virtual void Init()
    {
        BattleManager.Instance.GetParam(config, (param) => {
            this.param = param;
            scale = param.scale;

            if (model != null )
            {
                model.LoadModel();
            }
        });
    }
    public void OnComponentInitialize()
    {
        Clear();
    }

    public void OnComponentDestroy()
    {
       
    }

    public virtual void Clear()
    {
        id = 0;
        campflag = 0;
        name = "";
     
        type = 0;
        config = "";             //表的ID

        scale = 1;


        position = Vector3.zero;
        rotation = Quaternion.identity;

        machine.Clear();
        model = null;

        ResetProperty();

        this.Foreach((component) =>
        {
            IPoolObject poolObject = component as IPoolObject;
            if (poolObject != null && this != component)
            {
                ObjectPool.ReturnInstance(poolObject);
            }
        });
        this.RemoveAllComponents();

    }

    public void ResetProperty()
    {
        properties.SetProperty((uint)PropertyID.PRO_HP, 0);
        properties.SetProperty((uint)PropertyID.PRO_MAX_HP, 0);
        properties.SetProperty((uint)PropertyID.PRO_ATTACK, 0);
        properties.SetProperty((uint)PropertyID.PRO_DEFENSE, 0);
        properties.SetProperty((uint)PropertyID.PRO_MOVE_SPEED, 6f);
        properties.SetProperty((uint)PropertyID.PRO_ATTACK_DURATION, 2f);
        properties.SetProperty((uint)PropertyID.PRO_SEARCH_DISTANCE, 20f);
        properties.SetProperty((uint)PropertyID.PRO_ATTACK_DISTANCE, 10f);
        properties.SetProperty((uint)PropertyID.PRO_RADIUS, 1f);
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
            action.SetAgent(this);
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

    public List<State> GetActions(ActionType type)
    {
        return  machine.GetStates((int)type);
    }

    public void Remove(EntityAction action)
    {
        machine.Remove(action);
    }


    public  void OnUpdate(float deltaTime)
    {
        if (isPause)
        {
            return;
        }


        if (machine != null)
        {
            if (machine.current == null)
            {
                PlayAction(ActionType.Idle);
            }
            machine.OnUpdate(deltaTime);
            
        }
        this.Foreach((component) => {
            var update = component as IUpdate;
            if(update!=null)
            {
                update.OnUpdate(deltaTime);
            }
        });
    }

    public void UpdateProperty(PBMessage.BattleEntityProperty property)
    {
        if (property.ratio != 1)
        {
            properties.SetProperty( property.key, property.value * property.ratio);
        }
        else
        {
            properties.SetProperty(property.key, property.value);
        }
    }

    public  void UpdateProperty(List<PBMessage.BattleEntityProperty> properties)
    {
        for(int i = 0; i < properties.Count; ++i)
        {
            UpdateProperty(properties[i]);
        }
    }

    public void MoveTo(Vector3 destination, Vector3 velocity , bool done = true, Action<BattleEntity, Vector3> arriveAction = null, Action<BattleEntity, Vector3> failedAction = null)
    {

        var run = GetFirst(ActionType.Run);
        if (run != null)
        {
            run.ClearPath();
            run.AddPathPoint(destination, velocity, done, arriveAction, failedAction);
        }
        else
        {
            run = ObjectPool.GetInstance<EntityAction>();
            run.AddPathPoint(destination, velocity, done, arriveAction, failedAction);

            machine.ClearQueue();

            PlayAction(ActionType.Run, run);
        }
    }

    public void Attack(uint target)
    {
        var attack = ObjectPool.GetInstance<EntityAction>();
        attack.target = target;

        PlayAction(ActionType.Attack, attack);
    }

    public void DropBlood(int value)
    {
        if (active)
        {
                  
        }
    }

    
    public void Die()
    {
      
        isDie = true;
        EntityAction action = ObjectPool.GetInstance<EntityAction>();
        PlayAction(ActionType.Die,action,true);

    }

    public void Destroy()
    {
        Clear();
    }

    
    public void OnConstruct()
    {
        Clear();
    }
    
    public void OnDestruct()
    {
        Clear();
    }

    public void OnAgentEnter(State state)
    {       
        this.Foreach((component) => {

            var agent = component as IStateAgent;
            if (agent != null)
            {
                agent.OnAgentEnter(state);
            }
        });
    }

    public void OnAgentExcute(State state, float deltaTime)
    {
        this.Foreach((component) =>
        {
            var agent = component as IStateAgent;
            if (agent != null)
            {
                agent.OnAgentExcute(state, deltaTime);
            }
        });
    }

    public void OnAgentExit(State state)
    {
        this.Foreach((component) =>
        {
            var agent = component as IStateAgent;
            if (agent != null)
            {
                agent.OnAgentExit(state);
            }
        });
    }

    public void OnAgentCancel(State state)
    {
        this.Foreach((component) =>
        {
            var agent = component as IStateAgent;
            if (agent != null)
            {
                agent.OnAgentCancel(state);
            }
        });
    }

    public void OnAgentPause(State state)
    {
        this.Foreach((component) =>
        {
            var agent = component as IStateAgent;
            if (agent != null)
            {
                agent.OnAgentPause(state);
            }
        });
    }

    public void OnAgentResume(State state)
    {
        this.Foreach((component) =>
        {
            var agent = component as IStateAgent;
            if (agent != null)
            {
                agent.OnAgentResume(state);
            }
        });
    }

    public void OnAgentDestroy(State state)
    {
        this.Foreach((component) =>
        {
            var agent = component as IStateAgent;
            if (agent != null)
            {
                agent.OnAgentDestroy(state);
            }
        });

        EntityAction action = state as EntityAction;
        if(action!= null)
        {
            ObjectPool.ReturnInstance(action, action.GetType());
        }
    }

   
}
