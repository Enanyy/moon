using System;
using UnityEngine;



/// <summary>
/// String 的扩展
/// </summary>
public static class StringEx
{
    public static bool ToBoolEx(this string text,bool defaultValue = false)
    {
        bool result = defaultValue;

        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        bool.TryParse(text, out result);

        return result;
    }
    public static short ToInt16Ex(this string text,short defaultValue = 0)
    {
        short result = defaultValue;

        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        short.TryParse(text, out result);
        return result;

    }
    public static int ToInt32Ex(this string text,int defaultValue = 0)
    {
        int result = defaultValue;

        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        int.TryParse(text, out result);
        return result;

    }
    public static long ToInt64Ex(this string text, long defaultValue = 0)
    {
        long result = defaultValue;
        if (string.IsNullOrEmpty(text))
        {
            return result;
        }
        
        long.TryParse(text, out result);
        return result;
    }
    public static ushort ToUInt16Ex(this string text,ushort defaultValue = 0)
    {
        ushort result = defaultValue;
        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        ushort.TryParse(text, out result);
        return result;

    }
    public static uint ToUInt32Ex(this string text,uint defaultValue = 0)
    {
        uint result = defaultValue;

        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        uint.TryParse(text, out result);
        return result;

    }
    public static ulong ToUInt64Ex(this string text,ulong defaultValue = 0)
    {
        ulong result = defaultValue;
        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        ulong.TryParse(text, out result);
        return result;
    }
    public static float ToFloatEx(this string text,float defaultValue = 0)
    {
        float result = defaultValue;

        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

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

    public static string ReplaceEx(this string text, string beginFlag, string endFlag, string replace)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(beginFlag) || string.IsNullOrEmpty(endFlag))
        {
            return text;
        }
        int beginIndex = text.IndexOf(beginFlag);
        int endIndex = text.IndexOf(endFlag);
        if (beginIndex >= 0 && endIndex >= 0)
        {
            string part1 = text.Substring(0, beginIndex + beginFlag.Length + 1);
            string part2 = text.Substring(endIndex);

            return string.Format("{0}{1}{2}", part1, replace, part2);;
        }
        else
        {
            return text;
        }
    }
}