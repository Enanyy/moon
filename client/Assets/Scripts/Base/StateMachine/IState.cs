using UnityEngine;
using System.Collections;

public interface IState<T> where T:IStateAgent<T>
{
    T agent { get; set; }
    IState<T> parent { get; set; }
    void OnStateEnter();
    void OnStateExcute(float deltaTime);
    void OnStateExit();
    void OnStateCancel();
    void OnStatePause();
    void OnStateResume();
    void OnStateDestroy();
    void Clear();
}
