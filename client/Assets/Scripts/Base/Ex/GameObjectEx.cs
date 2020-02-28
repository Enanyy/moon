using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public static class GameObjectEx 
{
    public static void SetActiveEx(this GameObject go, bool active)
    {
        if(go&&go.activeSelf != active)
        {
            go.SetActive(active);
        }
    }

    public static void SetLayerEx(this GameObject gameObject, int layer, bool includeChildren = true)
    {
        if (gameObject != null)
        {
            gameObject.layer = layer;
            if (includeChildren)
            {
                Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
                for (int i = 0; i < transforms.Length; ++i)
                {
                    transforms[i].gameObject.layer = layer;
                }
            }
        }
    }

    public static T GetComponentEx<T>(this GameObject go, string path = null) where T : Component
    {
        if (go)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (go.TryGetComponent<T>(out T t))
                {
                    return t;
                }
            }
            else
            {
                Transform child = go.transform.Find(path);
                if (child)
                {
                    if (child.TryGetComponent<T>(out T t))
                    {
                        return t;
                    }
                }
            }
        }
        return null;
    }

    public static void SetTextEx(this GameObject go,  string content, string path = null)
    {
        Text text = go.GetComponentEx<Text>(path);
        if(text)
        {
            text.text = content;
        }
    }

}
