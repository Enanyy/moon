using UnityEngine;

public  interface IGameObject 
{
    Vector3 position { get; set; }
    Quaternion rotation { get; set; }
    float scale { get; set; }
}