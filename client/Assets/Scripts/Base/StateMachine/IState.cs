public interface IState
{
    void OnStateEnter();
    void OnStateExcute(float deltaTime);
    void OnStateExit();
    void OnStateCancel();
    void OnStatePause();
    void OnStateResume();
    void OnStateDestroy();
    void Clear();
}
