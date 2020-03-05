using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachine
{
    public State previous { get; private set; }
    public State current { get; private set; }
    public State next {
        get {
            if (mStateList.Count > 1)
            {
                return mStateList.First.Next.Value;
            }
            else { return null; }
        }
    }
    private LinkedList<State> mStateList = new LinkedList<State>();


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
  

    public StateMachine()
    {
        Clear();
    }

    public bool AddFirst(State state)
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

        DoNext();

        return true;
    }

    public bool AddLast(State state)
    {
        if(state == null)
        {
            return false;
        }
        state.SetStateMachine(this);
        mStateList.AddLast(state);
        return true;
    }

    public bool AddBefore(State node, State state)
    {
        if (state == null)
        {
            return false;
        }
        state.SetStateMachine(this);
        var last = mStateList.Last;
        var result = false;
        while (last!= null)
        {
            if (last.Value == node)
            {
                mStateList.AddBefore(last, state);
                result = true;
                break;
            }
            else
            {
                last = last.Previous;
            }
        }

        if (result == false)
        {
            if (mStateList.Count == 0)
            {
                mStateList.AddLast(state);
            }
            else
            {
                mStateList.AddAfter(mStateList.First, state);
            }
        }

        return true;
    }

    public bool AddAfter(State node, State state)
    {
        if (state == null)
        {
            return false;
        }
        state.SetStateMachine(this);
      
        var first = mStateList.First;
        var result = false;
        while (first!= null)
        {
            if (first.Value == node)
            {
                mStateList.AddAfter(first, state);
                result = true;
                break;
            }
            else
            {
                first = first.Next;
            }
        }

        if (result == false)
        {
            mStateList.AddLast(state);
        }
        return true;
    }

    public State GetFirst(int type)
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

    public State GetLast(int type)
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

    public List<State> GetStates(int type)
    {
        List<State> states = new List<State>();
        var it = mStateList.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.type == type)
            {
                states.Add(it.Current);
            }
        }

        return states;
    }

    public void Remove(State state)
    {
        if (state == null)
        {
            return;
        }
        var it = mStateList.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current== state)
            {
                mStateList.Remove(it.Current);
                break;
            }
        }
    }

    /*状态改变*/
    private bool DoNext()
    {
        //触发退出状态调用Exit方法
        if (current != null)
        {
            if(current.IsCancel())
            {
                current.OnStateCancel();
            }
            current.OnStateExit();
            mStateList.RemoveFirst();
        }

        if (previous != null)
        {
            previous.OnStateDestroy();
        }

        //保存上一个状态 
        previous = current;
        current = null;

        while (mStateList.Count > 0)
        {
            var state = mStateList.First.Value;
            if (state.IsValid()==false)
            {
                state.OnStateCancel();
                state.OnStateDestroy();
                mStateList.RemoveFirst();
            }
            else
            {
                if (state.duration <= 0)
                {
                    state.OnStateEnter();
                    state.OnStateExit();
                    state.OnStateDestroy();
                    mStateList.RemoveFirst();
                }
                else
                {
                    break;
                }
            }
        }

        //设置新状态为当前状态
        if(mStateList.Count > 0)
        {
            current = mStateList.First.Value;
            //进入当前状态调用Enter方法
            current.OnStateEnter();
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
            || (mStateList.Count > 1  && (mStateList.First.Next.Value.weight > current.weight || current.IsSkipping())))
        {
            DoNext();
        }
        if (current != null)
        {
            current.OnStateExcute(deltaTime);
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
            if (current.IsCancel())
            {
                current.OnStateCancel();
            }
            current.OnStateExit();
            current.OnStateDestroy();
        }
       
        if(previous!=null)
        {
            previous.OnStateDestroy();
        }
        
        var it = mStateList.GetEnumerator();
        while(it.MoveNext())
        {
            it.Current.OnStateCancel();
            it.Current.OnStateDestroy();
        }
        mStateList.Clear();
        previous = null;
        current = null;
    }

    
}