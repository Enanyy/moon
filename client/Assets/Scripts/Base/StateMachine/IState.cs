using UnityEngine;
using System.Collections;

public interface IState<T> where T:IStateAgent<T>
{
    T agent { get; set; }
    IState<T> parent { get; set; }
    void OnEnter();
    void OnExcute(float deltaTime);
    void OnExit();
    void OnCancel();
    void OnPause();
    void OnResume();
    void OnDestroy();
    void Clear();
}
