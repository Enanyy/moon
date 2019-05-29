using System;
public interface IComponent<T> where T:Components<T>
{
    T agent { get; set; }
    void OnUpdate(float deltaTime);
    void OnStart();
    void OnDestroy();
}

