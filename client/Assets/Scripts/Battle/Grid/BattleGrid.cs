using System.Collections.Generic;
using UnityEngine;

public class BattleTile : ITile
{
    public int index { get; set; }

    public int line { get; set; }
    public int column { get; set; }

    public Vector3 position { get; set; }

    public GameObject gameObject { get; private set; }

    public Color defaultColor = Color.white;

    public void Show(Transform parent,Mesh mesh, Material material)
    {
        if(gameObject== null)
        {
            string name = string.Format("{0}-({1},{2})", index, line, column);
            gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent);
            gameObject.transform.position = position;

            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            filter.mesh = mesh;
            renderer.material = material;
            collider.sharedMesh = mesh;

            //GameObject go = new GameObject("Text");
            //go.transform.SetParent(gameObject.transform);
            //go.transform.localPosition = Vector3.zero;
            //go.transform.localEulerAngles = new Vector3(90, 0, 0);
            //TextMesh text = go.AddComponent<TextMesh>();
            //text.anchor = TextAnchor.MiddleCenter;
            //text.alignment = TextAlignment.Center;
            //text.text = name;
            //text.color = Color.black;
            //text.fontSize = 7;

            SetColor(defaultColor);
        }
    }

    public void Select(bool value)
    {
        SetColor(value ? Color.green : defaultColor);
    }
    public void SetColor(Color color)
    {
        if (gameObject)
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            renderer.material.SetColor("_Color", color);
        }
    }
}

public class BattleGrid :RectGrid<BattleTile>
{
    private BattleGrid() { }
    private static BattleGrid _instance;
    public static BattleGrid Instance
    {
        get
        {
            if (_instance == null) _instance = new BattleGrid();
            return _instance;
        }
    }

    private GameObject mClick;

    private BattleTile mTile;

    private List<BattleTile> mCovers;

    private Material mMaterial;

    private Plane mPlane = new Plane(Vector3.up, Vector3.zero);

    public GameObject root { get; private set; }

    private Mesh mTileMesh;
    private bool mShowGrid = false;
    
    public bool showGrid
    {
        get { return mShowGrid; }
        set
        {
            mShowGrid = value;
            if(mShowGrid)
            {
                if(root == null)
                {
                    if (mTileMesh == null)
                    {
                        mTileMesh = GenerateTileMesh(tileWidth,tileHeight);
                    }
                    if (mMaterial == null)
                    {
                        mMaterial = Resources.Load<UnityEngine.Material>("r/material/tile");
                    }

                    root = new GameObject("Grid");
                    root.transform.position = original;
                    var it = tiles.GetEnumerator();
                    while(it.MoveNext())
                    {
                        it.Current.Value.Show(root.transform, mTileMesh, mMaterial);
                    }
                }
                else
                {
                    root.SetActive(true);
                }
            }
            else
            {
                if(root)
                {
                    root.SetActive(false);
                }
            }
        }
    }

    public override void Init(Vector3 original, int lines, int columns, float tileWidth, float tileHeight)
    {
        base.Init(original, lines, columns, tileWidth, tileHeight);

        BattleEntity entity = ObjectPool.GetInstance<BattleEntity>();
        entity.id = 1;
        entity.configid = 10003;
        entity.campflag = 1;
        entity.type = 1;

        BattleTile tile = TileAt(0, 0);
        entity.position = tile.position;

        BattleManager.Instance.AddEntity(entity);
        entity.active = true;

        
    }

    protected override void OnCreateTile(BattleTile t)
    {
        base.OnCreateTile(t);
  
        if(t.line % 2== 0)
        {
            t.defaultColor = t.column % 2 == 0 ? Color.white:Color.gray;
        }
        else
        {
            t.defaultColor = t.column % 2 == 1 ? Color.white : Color.gray;
        }

        if (mShowGrid)
        {
            if(mTileMesh == null)
            {
                mTileMesh = GenerateTileMesh(tileWidth,tileHeight);
            }
            if(mMaterial == null)
            {
                mMaterial = Resources.Load<UnityEngine.Material>("r/material/tile");
            }

            t.Show(root.transform,mTileMesh,mMaterial);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        BattleManager.Instance.Update(Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            mPlane.Raycast(ray, out distance);
            Vector3 point = ray.GetPoint(distance);

            if (mClick == null)
            {
                mClick = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mClick.transform.localScale = Vector3.one * 0.1f;
                MeshRenderer renderer = mClick.GetComponent<MeshRenderer>();
                renderer.material.SetColor("_Color", Color.red);
            }

            mClick.transform.position = point;


            var tile = TileAt(point);
            if (tile != null)
            {
                if (mTile != null)
                {
                    mTile.Select(false);
                }

                mTile = tile;
                mTile.Select(true);

                BattleEntity entity = BattleManager.Instance.GetEntity(1);
                if(entity!= null)
                {
                    EntityAction jump = ObjectPool.GetInstance<EntityAction>();
                    jump.AddPathPoint(mTile.position, Vector3.zero, true);

                    entity.PlayAction(ActionType.Jump, jump);
                }

                //if (mCovers != null)
                //{
                //    for (int i = 0; i < mCovers.Count; i++)
                //    {
                //        mCovers[i].Select(false);
                //    }
                //}

                //mCovers =TilesInDistance (mTile.index, 2);
                //for (int i = 0; i < mCovers.Count; i++)
                //{
                //    mCovers[i].Select(true);
                //}
            }
        }
    }
}