using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereCenter : MonoBehaviour
{
    private static SphereCenter mInstance;
    public static SphereCenter Instance
    {
        get
        {
            if(mInstance == null)
            {
                mInstance = FindObjectOfType<SphereCenter>();
            }
            return mInstance;
        }
    }

    public float radius = 100;
   
}

