using System;

public interface IStateAgent<T> where T : IStateAgent<T>
{
    void OnAgentEnter(State<T> state);
    void OnAgentExcute(State<T> state, float deltaTime);
    void OnAgentExit(State<T> state);
    void OnAgentCancel(State<T> state);
    void OnAgentPause(State<T> state);
    void OnAgentResume(State<T> state);
    void OnAgentDestroy(State<T> state);
}
