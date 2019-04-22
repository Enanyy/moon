using System;
using UnityEngine;



/// <summary>
/// String 的扩展
/// </summary>
public static class StringEx
{
    public static bool ToBoolEx(this string text)
    {
        bool result = false;

        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        bool.TryParse(text, out result);

        return result;
    }
    public static short ToInt16Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        short result = 0;
        short.TryParse(text, out result);
        return result;

    }
    public static int ToInt32Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        int result = 0;
        int.TryParse(text, out result);
        return result;

    }
    public static long ToInt64Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        long result = 0;
        long.TryParse(text, out result);
        return result;
    }
    public static ushort ToUInt16Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        ushort result = 0;
        ushort.TryParse(text, out result);
        return result;

    }
    public static uint ToUInt32Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        uint result = 0;
        uint.TryParse(text, out result);
        return result;

    }
    public static ulong ToUInt64Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        ulong result = 0;
        ulong.TryParse(text, out result);
        return result;
    }
    public static float ToFloatEx(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0f;
        }

        float result = 0;
        float.TryParse(text, out result);
        return result;
    }
    public static Bounds ToBoundsEx(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Bounds();
        }
        string v = text.Substring(1, text.Length - 2);
        string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
        if (values.Length == 6)
        {
            Vector3 center = new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
            Vector3 size = new Vector3(Convert.ToSingle(values[3]), Convert.ToSingle(values[4]), Convert.ToSingle(values[5]));
            return new Bounds(center, size);
        }
        return new Bounds();
    }

    public static Vector3 ToVector2Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Vector2();
        }
        string v = text.Substring(1, text.Length - 2);
        string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
        if (values.Length == 2)
        {
            return new Vector2(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]));
        }
        return new Vector2();
    }
    public static Vector3 ToVector3Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Vector3();
        }
        string v = text.Substring(1, text.Length - 2);
        string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
        if (values.Length == 3)
        {
            return new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
        }
        return new Vector3();
    }
    public static Vector4 ToVector4Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Vector4();
        }
        string v = text.Substring(1, text.Length - 2);
        string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
        if (values.Length == 4)
        {
            return new Vector4(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]), Convert.ToSingle(values[3]));
        }
        return new Vector4();
    }
    public static Color ToColorEx(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Color.white;
        }
        string v = text.Substring(1, text.Length - 2);
        string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
        if (values.Length == 4)
        {
            return new Color(Convert.ToSingle(values[0]) / 255f, Convert.ToSingle(values[1]) / 255f, Convert.ToSingle(values[2]) / 255f, Convert.ToSingle(values[3]) / 255f);
        }
        return Color.white;
    }

    public static T ToEnumEx<T>(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return default(T);
        }
        return (T)Enum.Parse(typeof(T), text);
    }
    public static Rect ToRectEx(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Rect();
        }
        string v = text.Substring(1, text.Length - 2);
        string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
        if (values.Length == 4)
        {
            return new Rect(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]), Convert.ToSingle(values[3]));
        }
        return new Rect();
    }
}