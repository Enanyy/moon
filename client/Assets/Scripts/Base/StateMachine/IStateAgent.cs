using System;

public interface IStateAgent 
{
    void OnAgentEnter(State state);
    void OnAgentExcute(State state, float deltaTime);
    void OnAgentExit(State state);
    void OnAgentCancel(State state);
    void OnAgentPause(State state);
    void OnAgentResume(State state);
    void OnAgentDestroy(State state);
}
