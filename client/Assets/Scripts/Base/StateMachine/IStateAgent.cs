using System;

public interface IStateAgent<T> where T : IStateAgent<T>
{
    void OnEnter(State<T> state);
    void OnExcute(State<T> state, float deltaTime);
    void OnExit(State<T> state);
    void OnCancel(State<T> state);
    void OnPause(State<T> state);
    void OnResume(State<T> state);
    void OnDestroy(State<T> state);
}
