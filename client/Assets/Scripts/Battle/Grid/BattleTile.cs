using UnityEngine;


public class BattleTile : ITile
{
    public TileIndex index { get; set; }

    public Vector3 position { get; set; }

    public GameObject gameObject { get; protected set; }

    public Color defaultColor = Color.white;

    public bool isValid = true;

    public BattleGridEntity entity;
    public virtual void Show(Transform parent, Mesh mesh, Material material)
    {
        if (gameObject == null)
        {
            string name = string.Format("{0}", index.ToString());
            gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent);
            gameObject.transform.position = position;

            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            filter.mesh = mesh;
            renderer.material = material;
            collider.sharedMesh = mesh;

            GameObject go = new GameObject("Text");
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = new Vector3(90, 0, 0);
            TextMesh text = go.AddComponent<TextMesh>();
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.text = index.ToString();
            text.color = Color.black;
            text.fontSize = 7;

            SetColor(defaultColor);
        }
    }

    public virtual void Select(bool value)
    {
        SetColor(value ? Color.green : defaultColor);
    }
    public virtual void SetColor(Color color)
    {
        if (gameObject)
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            renderer.material.SetColor("_Color", color);
        }
    }

    public virtual void SetColor()
    {
        SetColor(isValid?defaultColor:Color.black);
    }

    public virtual void Clear()
    {
        if (gameObject)
        {
            Object.DestroyImmediate(gameObject);
        }

        gameObject = null;
    }
}