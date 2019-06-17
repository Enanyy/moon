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
    Components<BattleEntity>, 
    IGameObject,
    IStateAgent<BattleEntity>,
    IPoolObject
{

    public uint id;             //唯一ID  
    public uint campflag;       //阵营
    public string name;         //头顶名字
  
    public uint type;            //实体类型（1为英雄，2为士兵 枚举:EMObjType)
    public string config;        //配置id

  

    public virtual Vector3 position { get; set; }
    public virtual Quaternion rotation { get; set; }
    public float scale { get; set; }

    public Dictionary<uint,IEntityProperty> properties { get; private set; }

    public StateMachine<BattleEntity> machine { get; private set; }


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
                    AddComponent(model);
                }
            }
            else
            {
                if (model != null)
                {
                    ObjectPool.ReturnInstance(model);
                    RemoveComponent(model);
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
            float radius = GetProperty<float>(PropertyID.PRO_RADIUS);

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
        properties = new Dictionary<uint, IEntityProperty>();
        Clear();
    }

    public virtual void Init()
    {
        BattleManager.Instance.GetParam(config, (param) => {
            this.param = param;
            scale = param.scale;
        });
    }

    public override void Clear()
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

        for (int i = 0; i < components.Count; ++i)
        {
            var component = components[i] as IPoolObject;
            if (component != null)
            {
                ObjectPool.ReturnInstance(component);

                RemoveComponent(component as IComponent<BattleEntity>);
            }
        }

        base.Clear();
      
    }

    public void ResetProperty()
    {
        SetProperty(PropertyID.PRO_HP, 0);
        SetProperty(PropertyID.PRO_MAX_HP, 0);
        SetProperty(PropertyID.PRO_ATTACK, 0);
        SetProperty(PropertyID.PRO_DEFENSE, 0);
        SetProperty(PropertyID.PRO_MOVE_SPEED, 6f);
        SetProperty(PropertyID.PRO_ATTACK_DURATION, 2f);
        SetProperty(PropertyID.PRO_SEARCH_DISTANCE, 20f);
        SetProperty(PropertyID.PRO_ATTACK_DISTANCE, 10f);
        SetProperty(PropertyID.PRO_RADIUS, 1f);
    }

    public void SetProperty<T>(PropertyID id,T value)
    {
        uint key = (uint)id;
        if(properties.ContainsKey(key)==false)
        {
            properties.Add(key, new EntityProperty<T>
            {
                value = value,
                defaultValue = value,
            });
        }
        else
        {
            var property = properties[key] as EntityProperty<T>;
            property.value = value;
        }
    }

    public T GetProperty<T>(PropertyID id, T defaultValue = default(T))
    {
        uint key = (uint) id;
        if (properties.ContainsKey(key))
        {
            var property = properties[key] as EntityProperty<T>;
            return property.value;
        }

        return defaultValue;
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

    public List<State<BattleEntity>> GetActions(ActionType type)
    {
        return  machine.GetStates((int)type);
    }

    public void Remove(EntityAction action)
    {
        machine.Remove(action);
    }


    public override void OnUpdate(float deltaTime)
    {
        if (isPause)
        {
            return;
        }

        base.OnUpdate(deltaTime);

        

        if (machine != null)
        {
            if (machine.current == null)
            {
                PlayAction(ActionType.Idle);
            }
            machine.OnUpdate(deltaTime);

            
        }
    }

    public void UpdateProperty(PBMessage.BattleEntityProperty property)
    {
        if (property.ratio != 1)
        {
            SetProperty((PropertyID) property.key, property.value * property.ratio);
        }
        else
        {
            SetProperty((PropertyID)property.key, property.value);
        }
    }

    public  void UpdateProperty(List<PBMessage.BattleEntityProperty> properties)
    {
        for(int i = 0; i < properties.Count; ++i)
        {
            UpdateProperty(properties[i]);
        }
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

    public override void Destroy()
    {
        Clear();
        base.Destroy();
    }

    
    public void OnCreate()
    {
        Clear();
    }
  
    public void OnReturn()
    {
        Clear();
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
