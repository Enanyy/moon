using System;
using System.Collections.Generic;
using UnityEngine;
public class BonePoint:MonoBehaviour
{
    public BoneType type;

    public static BonePoint GetBonePoint(Transform transform, BoneType type)
    {
        var bones = transform.GetComponentsInChildren<BonePoint>();
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

