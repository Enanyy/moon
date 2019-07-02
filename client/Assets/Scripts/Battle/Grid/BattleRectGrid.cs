using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.Rendering;

public class BattleRectTile : BattleTile
{
    public override void Show(Transform parent,Mesh mesh, Material material)
    {
        base.Show(parent, mesh, material);
        
    }
 
}

public class BattleRectGrid :RectGrid<BattleRectTile>
{
    private BattleRectGrid() { }
    private static BattleRectGrid _instance;
    public static BattleRectGrid Instance
    {
        get
        {
            if (_instance == null) _instance = new BattleRectGrid();
            return _instance;
        }
    }

    private GameObject mClick;

    private BattleRectTile mTile;

    private Material mMaterial;

    private Plane mPlane = new Plane(Vector3.up, Vector3.zero);

    public GameObject root { get; private set; } = null;

    public bool initialized { get; private set; } = false;

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
                        AssetManager.Instance.LoadAsset<Material>("tile.mat", (asset) => {

                            if (root == null)
                            {
                                mMaterial = asset.assetObject;
                                root = new GameObject("Grid");
                                root.transform.position = original;
                                var it = tiles.GetEnumerator();
                                while (it.MoveNext())
                                {
                                    it.Current.Value.Show(root.transform, mTileMesh, mMaterial);
                                }
                            }
                        });
                    }
                    else
                    {
                        root = new GameObject("Grid");
                        root.transform.position = original;
                        var it = tiles.GetEnumerator();
                        while (it.MoveNext())
                        {
                            it.Current.Value.Show(root.transform, mTileMesh, mMaterial);
                        }
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

        initialized = true;

        BattleGridEntity entity = ObjectPool.GetInstance<BattleGridEntity>();
        entity.id = 1;
        entity.heroid = 1;
        entity.campflag = 1;
        entity.type = 1;
        entity.grid = BattleGrid.Rect;
        BattleRectTile tile = TileAt(0, 0);
        entity.position = tile.position;

        BattleManager.Instance.AddEntity(entity);
        entity.active = true;

        
    }

    protected override BattleRectTile CreateTile(TileIndex index, Vector3 position)
    {
        var t = base.CreateTile(index,position);
  
        if(index.x % 2== 0)
        {
            t.defaultColor = index.z % 2 == 0 ? Color.white:Color.gray;
        }
        else
        {
            t.defaultColor = index.z % 2 == 1 ? Color.white : Color.gray;
        }

        if (mShowGrid)
        {
            if(mTileMesh == null)
            {
                mTileMesh = GenerateTileMesh(tileWidth,tileHeight);
            }
            if(mMaterial == null)
            {
                AssetManager.Instance.LoadAsset<Material>("tile.mat", (asset) => {

                    mMaterial = asset.assetObject;
                    t.Show(root.transform, mTileMesh, mMaterial);

                });
            }
            else
            {
                t.Show(root.transform, mTileMesh, mMaterial);
            }

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

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            mPlane.Raycast(ray, out distance);
            Vector3 point = ray.GetPoint(distance);
            var tile = TileAt(point);
            if (tile != null)
            {
                tile.isValid = !tile.isValid;
                tile.SetColor(!tile.isValid ? Color.black : tile.defaultColor);
            }
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
                if (mEntity != null)
                {
                    EntityAction jump = mEntity.GetFirst(ActionType.Jump);

                    if (Chess.Instance.jump)
                    {
                        if (jump == null)
                        {
                            jump = ObjectPool.GetInstance<EntityAction>();
                            jump.AddPathPoint(mTile.position, Vector3.zero, true, (entity, destination) =>
                                {
                                    var p = TileAt(destination);
                                    if (p != null)
                                    {
                                        p.SetColor();
                                    }
                                },
                                (entity, destination) =>
                                {
                                    var p = TileAt(destination);
                                    if (p != null)
                                    {
                                        p.SetColor();
                                    }
                                });
                            mEntity.PlayAction(ActionType.Jump, jump);
                        }
                        else
                        {
                            jump.AddPathPoint(mTile.position, Vector3.zero, true, (entity, destination) =>
                                {
                                    var p = TileAt(destination);
                                    if (p != null)
                                    {
                                        p.SetColor();
                                    }
                                },
                                (entity, destination) =>
                                {
                                    var p = TileAt(destination);
                                    if (p != null)
                                    {
                                        p.SetColor();
                                    }
                                });
                        }
                    }
                    else
                    {
                        var position = mEntity.position;
                        if (jump != null && jump.paths.Count > 0)
                        {
                            position = jump.paths.Last.Value.destination;
                        }

                        var result = FindPath(TileAt(position), mTile,
                            (tile) => { return tile.isValid; },
                            (tile) => { return Neighbours(tile, RectNeighbors.All); },
                            GetCostValue);

                        if (jump == null)
                        {
                            jump = ObjectPool.GetInstance<EntityAction>();
                            while (result.Count > 0)
                            {
                                var path = result.Pop();
                                jump.AddPathPoint(path.position, Vector3.zero, false, (entity, destination) =>
                                    {
                                        var p = TileAt(destination);
                                        if (p != null)
                                        {
                                            p.SetColor();
                                        }
                                    },
                                    (entity, destination) =>
                                    {
                                        var p = TileAt(destination);
                                        if (p != null)
                                        {
                                            p.SetColor();
                                        }
                                    });
                            }

                            mEntity.PlayAction(ActionType.Jump, jump);
                        }
                        else
                        {
                            while (result.Count > 0)
                            {
                                var path = result.Pop();
                                jump.AddPathPoint(path.position, Vector3.zero, false, (entity, destination) =>
                                {
                                    var p = TileAt(destination);
                                    if (p != null)
                                    {
                                        p.SetColor();
                                    }
                                },
                                (entity, destination) =>
                                {
                                    var p = TileAt(destination);
                                    if (p != null)
                                    {
                                        p.SetColor();
                                    }
                                });
                            }
                        }
                    }
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

                    AssetManager.Instance.LoadAsset<Material>("arrow.mat", (asset) => {

                        mPathRenderer.material = asset.assetObject;
                    });

                    
                    mPathRenderer.startWidth = 1;
                    mPathRenderer.endWidth = 1;
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

                        BattleRectTile t = TileAt(position);


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

                            if (Chess.Instance.jump == false)
                            {
                                var result = FindPath(t,mTile, 
                                    (o) => { return o.isValid;},
                                    (o) => { return Neighbours(o, RectNeighbors.All);},
                                    GetCostValue
                                );

                                var it = tiles.GetEnumerator();
                                while (it.MoveNext())
                                {
                                    it.Current.Value.SetColor();
                                }

                                var jumps = mEntity.GetActions(ActionType.Jump);
                                if (jumps != null)
                                {
                                    var j = jumps.GetEnumerator();
                                    while (j.MoveNext())
                                    {
                                        var jump = j.Current as EntityAction;
                                        var d = jump.paths.GetEnumerator();
                                        while (d.MoveNext())
                                        {
                                            var dest = TileAt(d.Current.destination);
                                            if (dest != null)
                                            {
                                                dest.SetColor(Color.green);
                                            }
                                        }
                                    }
                                }

                                var r = result.GetEnumerator();
                                while (r.MoveNext())
                                {
                                    r.Current.SetColor(Color.green);
                                }
                            }


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