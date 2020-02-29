using System;
using System.Collections.Generic;

public class ActionPlugin : IState,IPoolObject
{
    public BattleEntity agent { get; set; }
   

    public EntityAction action { get; set; }

    public EntityParamPlugin param { get; private set; }


    public virtual void Init(EntityParamPlugin param)
    {
        this.param = param;
    }

    public virtual void OnStateCancel()
    {

    }

    

    public virtual void OnStateEnter()
    {

    }

    public virtual void OnStateExcute(float deltaTime)
    {

    }

    public virtual void OnStateExit()
    {

    }

    public virtual void OnStatePause()
    {

    }

    public virtual void OnStateResume()
    {

    }

    public virtual void Clear()
    {

    }

    public virtual void OnDestruct()
    {

    }

    public virtual void OnConstruct()
    {
       
    }
    public virtual void OnStateDestroy() 
    {

    }
}

