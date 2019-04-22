using System;
using System.Collections.Generic;
using UnityEngine;
public static class RectEx
{
    public static string ToStringEx(this Rect rect)
    {
        return string.Format("({0},{1},{2},{3})", rect.x, rect.y, rect.width, rect.height);
    }
}

