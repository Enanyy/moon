using UnityEngine;

public static class GameObjectEx 
{
    public static void SetLayer(this GameObject gameObject, int layer, bool includeChildren = true)
    {
        if(gameObject!= null)
        {
            gameObject.layer = layer;
            if(includeChildren)
            {
                Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
                for (int i = 0; i < transforms.Length; ++i)
                {
                    transforms[i].gameObject.layer = layer;
                }
            }
        }
    }
    
}