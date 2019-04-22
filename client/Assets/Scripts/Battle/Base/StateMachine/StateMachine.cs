using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachine<T> where T: IStateAgent<T>
{
    public State<T> previous { get; private set; }
    public State<T> current { get; private set; }
    public State<T> next {
        get {
            if (mStateList.Count > 1)
            {
                return mStateList.First.Next.Value;
            }
            else { return null; }
        }
    }
    private LinkedList<State<T>> mStateList = new LinkedList<State<T>>();


    private bool mPause;
    public bool isPause {
        get { return mPause; }
        set
        {
            mPause = value;
            
            if(current != null)
            {
                current.isPause = mPause;
            }
        }
    }
    public T agent { get; private set; }


    public StateMachine(T agent)
    {
        this.agent = agent;
        Clear();
    }

    public bool AddFirst(State<T> state)
    {
        if(state == null)
        {
            return false;
        }
        state.SetStateMachine(this);
        if (mStateList.Count == 0)
        {
            mStateList.AddLast(state);
        }
        else
        {
            mStateList.AddAfter(mStateList.First, state);
        }
        return true;
    }

    public bool AddLast(State<T> state)
    {
        if(state == null)
        {
            return false;
        }
        state.SetStateMachine(this);
        mStateList.AddLast(state);
        return true;
    }

    public State<T> GetFirst(int type)
    {
        var it = mStateList.First;
        while (it != null)
        {
            if (it.Value.type == type)
            {
                return it.Value;
            }
            else
            {
                it = it.Next;
            }
        }
        return null;
    }

    public State<T> GetLast(int type)
    {
        var it = mStateList.Last;
        while (it != null)
        {
            if (it.Value.type == type)
            {
                return it.Value;
            }
            else
            {
                it = it.Previous;
            }
        }
        return null;
    }

    /*状态改变*/
    private bool DoNext()
    {
        //触发退出状态调用Exit方法
        if (current != null)
        {
            if(current.isCancel)
            {
                current.OnCancel();
            }
            current.OnExit();
            mStateList.RemoveFirst();
        }

        if (previous != null)
        {
            previous.OnDestroy();
        }

        //保存上一个状态 
        previous = current;
        current = null;

        while (mStateList.Count > 0)
        {
            if(mStateList.First.Value.IsValid()==false)
            {
                mStateList.RemoveFirst();
            }
            else
            {
                break;
            }
        }

        //设置新状态为当前状态
        if(mStateList.Count > 0)
        {
            current = mStateList.First.Value;
            //进入当前状态调用Enter方法
            current.OnEnter();
            return true;
        }
       
        return false;
    }

    public virtual void OnUpdate(float deltaTime)
    {
        if(mPause)
        {
            return;
        }

        if (current == null 
            || current.isDone 
            || (mStateList.Count > 1  && mStateList.First.Next.Value.weight > current.weight))
        {
            DoNext();
        }
        if (current != null)
        {
            current.OnExcute(deltaTime);
        }
    }

    public void RevertPreviousState()
    {
        if(previous!=null)
        {
            if (mStateList.Count > 0)
            {
                mStateList.AddAfter(mStateList.First, previous);
                DoNext();
            }
            else
            {
                mStateList.AddLast(previous);
            }
        }
    }

   

    public virtual void Clear()
    {
        if(current != null)
        {
            if (current.isCancel)
            {
                current.OnCancel();
            }
            current.OnExit();
        }
       
        if(previous!=null)
        {
            previous.OnDestroy();
        }
        
        var it = mStateList.GetEnumerator();
        while(it.MoveNext())
        {
            it.Current.OnDestroy();
        }
        mStateList.Clear();
        previous = null;
        current = null;
    }

    
}