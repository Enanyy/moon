using System;
using System.Collections.Generic;
using UnityEngine;
public class BoneObject:MonoBehaviour
{
    public BonePoint type;

    public static BoneObject GetBone(Transform transform, BonePoint type)
    {
        var bones = transform.GetComponentsInChildren<BoneObject>();
        for (int i = 0; i < bones.Length; ++i)
        {
            if (bones[i].type == type)
            {
                return bones[i];
            }
        }
        return null;
    }
}

