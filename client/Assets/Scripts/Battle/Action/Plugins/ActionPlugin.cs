using System;
using System.Collections.Generic;

public class ActionPlugin : IState<BattleEntity>,IPoolObject
{
    public BattleEntity agent { get; set; }
    public IState<BattleEntity> parent { get; set; }

    public EntityAction action { get { return parent as EntityAction; } }

    public bool isPool
    {
        get;set;
    }

    public virtual void OnCancel()
    {

    }

    

    public virtual void OnEnter()
    {

    }

    public virtual void OnExcute(float deltaTime)
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void OnPause()
    {

    }

    public virtual void OnResume()
    {

    }

    public virtual void Clear()
    {

    }

    public virtual void OnDestroy()
    {

    }

    public void OnCreate()
    {
       
    }

    public void OnReturn()
    {
        
    }
}

