using System;
using System.Collections.Generic;

public class State<T>: IState<T> where T : IStateAgent<T>
{
    public State()
    {
        Clear();
    }
    public StateMachine<T> machine { get; private set; }
    public IState<T> parent { get; set; }

    /// <summary>
    /// 状态类型
    /// </summary>
    public int type { get; set; }
    
    public T agent { get;  set; }

    /// <summary>
    /// 权重
    /// </summary>
    public int weight { get; set; }
    /// <summary>
    /// 持续时长
    /// </summary>
    public float duration { get; set; }
    /// <summary>
    /// 执行时长
    /// </summary>
    public float time { get; private set; }
    /// <summary>
    /// 执行速度
    /// </summary>
    public float speed { get; set; }
    /// <summary>
    /// 是否执行完毕
    /// </summary>
    public bool isDone { get; private set; }

    /// <summary>
    /// 是否取消执行的
    /// </summary>
    public bool isCancel { get { return time < duration; } }
    /// <summary>
    /// 暂停
    /// </summary>
    private bool mPause;
    public bool isPause
    {
        get { return mPause; }
        set
        {
            if (mPause && value == false)
            {
                OnStateResume();
            }
            else if (mPause == false && value)
            {
                OnStatePause();
            }
            mPause = value;
        }
    }
    /// <summary>
    /// 子状态
    /// </summary>
    protected List<IState<T>> mSubStateList = new List<IState<T>>();
   

    /// <summary>
    /// 完成当前状态
    /// </summary>
    public void Done()
    {
        isDone = true;
    }

    /// <summary>
    /// 是否可执行的
    /// </summary>
    /// <returns></returns>
    public virtual bool IsValid() { return true; }

    /// <summary>
    /// 添加子状态
    /// </summary>
    /// <param name="state"></param>
    public void AddSubState(IState<T> state)
    {
        if(mSubStateList.Contains(state)==false)
        {
            state.agent = agent;
            state.parent = this;
            mSubStateList.Add(state);
        }
    }
   
    /// <summary>
    /// 设置状态机
    /// </summary>
    /// <param name="varMachine"></param>
    public void SetStateMachine(StateMachine<T> varMachine)
    {
        machine = varMachine;
        SetAgent(machine.agent);
    }
    /// <summary>
    /// 设置状态代理
    /// </summary>
    /// <param name="agent"></param>
    public virtual void SetAgent(T agent)
    {
        this.agent = agent;
        for(int i = 0; i < mSubStateList.Count;++i)
        {
            mSubStateList[i].agent = agent;
        }
    }
    /// <summary>
    /// 进入状态
    /// </summary>
    public virtual void OnStateEnter()
    {
        if(agent!=null)
        {
            agent.OnAgentEnter(this);
        }
        for(int i = 0; i <mSubStateList.Count;++i)
        {
            mSubStateList[i].OnStateEnter();
        }
    }
    /// <summary>
    /// 状态取消
    /// </summary>
    public virtual void OnStateCancel()
    {
        if (agent != null)
        {
            agent.OnAgentCancel(this);
        }
        for (int i = 0; i < mSubStateList.Count; ++i)
        {
            mSubStateList[i].OnStateCancel();
        }
    }
    /// <summary>
    /// 状态执行
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void OnStateExcute(float deltaTime)
    {
        if (time < duration)
        {
            time += deltaTime * speed;
            if(time > duration)
            {
                Done();
            }
        }
        if (agent != null)
        {
            agent.OnAgentExcute(this,deltaTime);
        }
        for (int i = 0; i < mSubStateList.Count; ++i)
        {
            mSubStateList[i].OnStateExcute(deltaTime);
        }
    }
    /// <summary>
    /// 状态退出
    /// </summary>
    public virtual void OnStateExit()
    {
        if (agent != null)
        {
            agent.OnAgentExit(this);
        }
        for (int i = 0; i < mSubStateList.Count; ++i)
        {
            mSubStateList[i].OnStateExit();
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public virtual void OnStatePause()
    {
        if (agent != null)
        {
            agent.OnAgentPause(this);
        }
        for (int i = 0; i < mSubStateList.Count; ++i)
        {
            mSubStateList[i].OnStatePause();
        }
    }
    /// <summary>
    /// 恢复
    /// </summary>
    public virtual void OnStateResume()
    {
        if (agent != null)
        {
            agent.OnAgentResume(this);
        }
        for (int i = 0; i < mSubStateList.Count; ++i)
        {
            mSubStateList[i].OnStateResume();
        }
    }

    public virtual void OnStateDestroy()
    {
        if (agent != null)
        {
            agent.OnAgentDestroy(this);
        }
        for (int i = 0; i < mSubStateList.Count; ++i)
        {
            mSubStateList[i].OnStateDestroy();
        }
    }

    /// <summary>
    /// 重置
    /// </summary>
    public virtual void Clear()
    {
        weight = 0;
        time = 0;
        duration = 0;
        speed = 1;
        isDone = false;
        mPause = false;
        agent = default(T);
        parent = null;
        for(int i = 0; i <mSubStateList.Count; ++i)
        {
            mSubStateList[i].Clear();
        }
        mSubStateList.Clear();
    }
}
