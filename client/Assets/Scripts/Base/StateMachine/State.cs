using System;
using System.Collections.Generic;

public class State:IState
{
    public State()
    {
        Clear();
    }
    public StateMachine machine { get; private set; }
  
    /// <summary>
    /// 状态类型
    /// </summary>
    public int type { get; set; }
   

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
    protected List<IState> mSubStateList = new List<IState>();
   

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
    public void AddSubState(IState state)
    {
        if(mSubStateList.Contains(state)==false)
        {
            mSubStateList.Add(state);
        }
    }
   
    /// <summary>
    /// 设置状态机
    /// </summary>
    /// <param name="varMachine"></param>
    public void SetStateMachine(StateMachine varMachine)
    {
        machine = varMachine;    
    }
   
    /// <summary>
    /// 进入状态
    /// </summary>
    public virtual void OnStateEnter()
    {    
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
        for (int i = 0; i < mSubStateList.Count; ++i)
        {
            mSubStateList[i].OnStateResume();
        }
    }

    public virtual void OnStateDestroy()
    {     
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
        for(int i = 0; i <mSubStateList.Count; ++i)
        {
            mSubStateList[i].Clear();
        }
        mSubStateList.Clear();
    }
}
