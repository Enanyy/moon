using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BattleHexTile : ITile
{
    public TileIndex index { get; set; }

    public Vector3 position { get; set; }

    public GameObject gameObject { get; private set; }

    public Color defaultColor = Color.white;

    public void Show(Transform parent,Mesh mesh, Material material,float radius, HexOrientation hexOrientation)
    {
        if(gameObject== null)
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

            LineRenderer lines = gameObject.AddComponent<LineRenderer>();
            lines.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            lines.receiveShadows = false;

            lines.startWidth = 0.1f;
            lines.endWidth = 0.1f;
            lines.startColor = Color.black;
            lines.endColor = Color.black;

            lines.material = material;
           
            lines.positionCount = 7;

            for (int vert = 0; vert <= 6; vert++)
                lines.SetPosition(vert, HexGrid<BattleHexTile>.Corner(gameObject.transform.position, radius, vert, hexOrientation));

            GameObject go = new GameObject("Text");
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = new Vector3(90, 0, 0);
            TextMesh text = go.AddComponent<TextMesh>();
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.text = name;
            text.color = Color.black;
            text.fontSize = 7;

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

    public void Clear()
    {
        if (gameObject)
        {
            Object.DestroyImmediate(gameObject);
        }

        gameObject = null;
    }
}

public class BattleHexGrid :HexGrid<BattleHexTile>
{
    private BattleHexGrid() { }
    private static BattleHexGrid _instance;
    public static BattleHexGrid Instance
    {
        get
        {
            if (_instance == null) _instance = new BattleHexGrid();
            return _instance;
        }
    }

    private GameObject mClick;

    private BattleHexTile mTile;

    private List<BattleHexTile> mCovers;

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
                        mTileMesh = GenerateHexMesh(radius,orientation);
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
                        it.Current.Value.Show(root.transform, mTileMesh, mMaterial, radius,orientation);
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

    public override void Init(Vector3 original, HexGridShape shape, int lines, int columns, float radius,
        HexOrientation orientation)
    {
        base.Init(original, shape, lines, columns, radius, orientation);

        BattleEntity entity = ObjectPool.GetInstance<BattleEntity>();
        entity.id = 1;
        entity.configid = 10002;
        entity.campflag = 1;
        entity.type = 1;

        BattleHexTile tile = TileAt(0, 0);
        entity.position = tile.position;

        BattleManager.Instance.AddEntity(entity);
        entity.active = true;

        
    }

    protected override BattleHexTile CreateTile(TileIndex index, Vector3 position)
    {
        var t = base.CreateTile(index,position);

        if (index.x % 2 == 0)
        {
           // t.defaultColor = index.z % 2 == 0 ? Color.white : Color.gray;
           t.defaultColor = Color.white;
           
        }
        else
        {
            //t.defaultColor = index.z % 2 == 1 ? Color.white : Color.gray;
            t.defaultColor = Color.gray;
            
        }

        if (mShowGrid)
        {
            if(mTileMesh == null)
            {
                mTileMesh = GenerateHexMesh(radius,orientation);
            }
            if(mMaterial == null)
            {
                mMaterial = Resources.Load<UnityEngine.Material>("r/material/tile");
            }

            t.Show(root.transform, mTileMesh, mMaterial, radius, orientation);
        }
        return t; 
    }

    private List<Vector3> mPathPoints = new List<Vector3>();

    private bool mShowPath = false;

    private BattleEntity mEntity;

    private LineRenderer mPathRenderer;

    // Update is called once per frame
    public void Update()
    {
        BattleManager.Instance.Update(Time.deltaTime);

        if (mEntity == null)
        {
            mEntity = BattleManager.Instance.GetEntity(1);
        }

        if (mEntity == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mShowPath = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mShowPath = false;
         
            if (mTile != null)
            {
               
                mTile.Select(false);
     
                if (mEntity != null)
                {
                    EntityAction jump = ObjectPool.GetInstance<EntityAction>();
                    jump.AddPathPoint(mTile.position, Vector3.zero, true);

                    mEntity.PlayAction(ActionType.Jump, jump);
                }
            }

            mTile = null;
        }

        if (mShowPath)
        {
            if (mPathRenderer == null)
            {
                if (root)
                {
                    mPathRenderer = root.GetComponent<LineRenderer>();
                    if (mPathRenderer == null)
                    {
                        mPathRenderer = root.AddComponent<LineRenderer>();

                    }
                    mPathRenderer.material = Resources.Load<Material>("r/material/arrow");
                    mPathRenderer.startWidth = 1f;
                    mPathRenderer.endWidth = 1f;
                    mPathRenderer.startColor = Color.yellow;
                    mPathRenderer.endColor = Color.yellow;
                    mPathRenderer.receiveShadows = false;
                    mPathRenderer.shadowCastingMode = ShadowCastingMode.Off;
                }
            }

            if (mPathRenderer != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float distance;
                mPlane.Raycast(ray, out distance);
                Vector3 point = ray.GetPoint(distance);
                var tile = TileAt(point);
                if (tile == null)
                {
                    int minDistance = -1;
                    var it = tiles.GetEnumerator();
                    while (it.MoveNext())
                    {
                        int d = Distance(point, it.Current.Value.position);
                        if (minDistance == -1 || d < minDistance)
                        {
                            tile = it.Current.Value;
                            minDistance = d;
                        }
                    }
                }
                if (tile != null)
                {
                    if (mTile == null || (tile.index != mTile.index))
                    {
                        Vector3 position = mEntity.position;
                        var run = mEntity.GetLast(ActionType.Jump);
                        if (run != null)
                        {
                            if (run.paths.Count > 0)
                            {
                                position = run.paths.Last.Value.destination;
                            }
                        }
                        ITile t = TileAt(position);

                        if (t != null && t.index == tile.index)
                        {
                            ClearPath();

                        }
                        else
                        {
                            if (mTile != null)
                            {
                                mTile.Select(false);
                            }

                            mTile = tile;

                            mTile.Select(true);                          

                            BattleBezierPath.GetPath(position,
                                mTile.position,
                                0.5f,
                                ActionJumpPlugin.SPEED,
                                ActionJumpPlugin.MINHEIGHT,
                                ActionJumpPlugin.MAXHEIGHT,
                                ActionJumpPlugin.GRAVITY,
                                ref mPathPoints);


                            mPathRenderer.positionCount = mPathPoints.Count;
                            mPathRenderer.SetPositions(mPathPoints.ToArray());
                        }
                    }
                }
                else
                {
                    ClearPath();

                }

            }
        }
        else
        {
            ClearPath();
        }
    }

    private void ClearPath()
    {
        if (mTile != null)
        {
            mTile.Select(false);
        }
        mTile = null;
        if (mPathRenderer != null)
        {
            mPathRenderer.positionCount = 0;
        }
    }
}