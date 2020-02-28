using UnityEngine;
using UnityEngine.UI;

public static class ComponentEx 
{
   public static void SetActiveEx(this Component component,bool active)
    {
        if(component && component.gameObject.activeSelf !=active)
        {
            component.gameObject.SetActive(active);
        }
    }

    public static T GetComponentEx<T>(this Component component,string path = null) where T:Component
    {
        if(component)
        {
            if(string.IsNullOrEmpty(path))
            {
                if (component.TryGetComponent<T>(out T t))
                {
                    return t;
                }
            }
            else
            {
                Transform child = component.transform.Find(path);
                if(child)
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

    public static void SetTextEx(this Component component, string content, string path = null)
    {
        Text text = component.GetComponentEx<Text>(path);
        if (text)
        {
            text.text = content;
        }
    }
}