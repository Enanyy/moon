using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereCenter : MonoBehaviour
{
    public static SphereCenter Instance;

    void Awake()
    {
        Instance = this;
    }
}

