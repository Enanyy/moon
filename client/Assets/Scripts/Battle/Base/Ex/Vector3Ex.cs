using System;
using UnityEngine;

public static class Vector3Ex
{
    public static string ToStringEx(this Vector3 value)
    {
        return string.Format("({0},{1},{2})", value.x, value.y, value.z);
    }
}

