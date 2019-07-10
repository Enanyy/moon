using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ScalingSphere : MonoBehaviour {

    
   
    public float SphereRadius = 1;
    public int SphereDetail = 0;


    public Material material;

    private SphereGrid grid;

    public int range = 1;

    // Use this for initialization
    void Awake () {

        
        click = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        click.GetComponent<SphereCollider>().enabled = false;

        grid = new SphereGrid();

        string path = Application.dataPath + "/grid.txt";
        if (File.Exists(path))
        {
            string text = File.ReadAllText(path);
            string[] indexs = text.Split(';');
            for (int i = 0; i < indexs.Length; i++)
            {
                grid.blackList.Add(indexs[i].ToInt32Ex());
            }
        }

        grid.Init(SphereRadius,SphereDetail);
        grid.SetRoot(transform);

        //grid.Show(material);


        for (int i = 0; i < grid.roots.Count; ++i)
        {
            Color c = Color.white;
            switch (i % 4)
            {
                case 1: c = Color.blue; break;
                case 2: c = Color.grey; break;
                case 3: c = Color.yellow; break;
            }
            SetColor(grid.roots[i], c);

            //ShowPoint(grid.roots[i].a);
            //ShowPoint(grid.roots[i].b);
            //ShowPoint(grid.roots[i].c);

        }

    }
    private Tile mSelectTile;
    private GameObject click;

    private static Material mLineMaterial;

    private static Material lineMaterial
    {
        get
        {
            if (mLineMaterial == null)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");

                mLineMaterial = new Material(shader);

                mLineMaterial.hideFlags = HideFlags.HideAndDontSave;

                // Turn on alpha blending

                mLineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);

                mLineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

                // Turn backface culling off

                mLineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);

                // Turn off depth writes

                mLineMaterial.SetInt("_ZWrite", 0);
            }

            return mLineMaterial;
        }
    }

    public void OnRenderObject()
    {
        if (grid != null)
        {
            grid.GLDraw(lineMaterial);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (grid != null)
            {
                var tile = GetMousePositionTile();


                if (tile != null)
                {
                    if (mSelectTile != null)
                    {
                        mSelectTile.SetColor(mSelectTile.isValid ? mSelectTile.defaultColor : Color.black);
                    }

                    tile.SetColor(Color.green);

                    mSelectTile = tile;
                }
                else
                {
                    if (mSelectTile != null)
                    {
                        mSelectTile.SetColor(mSelectTile.isValid ? mSelectTile.defaultColor : Color.black);
                    }

                    mSelectTile = null;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (grid != null && mSelectTile != null)
            {

                var tile = GetMousePositionTile();

                if (tile != null)
                {
                    var points = grid.FindPath(mSelectTile,
                        tile,
                        (t) => { return t.isValid; }
                    );

                    if (points != null)
                    {
                        while (points.Count > 0)
                        {
                            var p = points.Pop();
                            p.SetColor(Color.green);
                        }
                    }
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mSelectTile != null)
            {
                mSelectTile.isValid = !mSelectTile.isValid;
                mSelectTile.SetColor(mSelectTile.isValid ? mSelectTile.defaultColor : Color.black);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            controll = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            controll = false;
        }
    }

    Tile GetMousePositionTile()
    {
        if (grid != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Default")))
            {
                Vector3 point = hit.point;

              
                var tile = grid.TileAt(point);

                click.transform.position = point;

                return tile;
            }

        }

        return null;
    }

    List<Tile> GetMousePositionTiles(int r)
    {
        if (grid != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Default")))
            {
                Vector3 point = hit.point;


                var tiles = grid.TilesInRange(point,r);

                click.transform.position = point;

                return tiles;
            }

        }

        return null;
    }

    void ShowPoint(Vector3 position)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = position;
    }


    void SetColor(Tile tile, Color color)
    {
        if (tile.children.Count > 0)
        {
            for (int i = 0; i < tile.children.Count; ++i)
            {
                SetColor(tile.children[i],color);
            }
        }
        else
        {
            tile.defaultColor = color;
            tile.color = color;
        }
    }

    [ContextMenu("Save")]
    void Save()
    {
        if (grid != null)
        {
            //string xml = grid.ToXml();

            //string path = Application.dataPath + "/grid.xml";
            //if (File.Exists(path))
            //{
            //    File.Delete(path);
            //}
            //File.WriteAllText(path,xml);
            StringBuilder builder = new StringBuilder();
            var it = grid.tiles.GetEnumerator();
            while (it.MoveNext())
            {
                if (it.Current.isValid == false)
                {
                    builder.AppendFormat("{0};", it.Current.index);
                }
            }

            string path = Application.dataPath + "/grid.txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, builder.ToString());
        }
    }


    private bool controll = false;

    public int brushSize = 2;


    void OnMouseDrag()     //鼠标拖拽时的操作// 
    {
        if (controll)
        {
            var tiles = GetMousePositionTiles(brushSize);
            if (tiles != null)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    var tile = tiles[i];
                    tile.isValid = false;

                    tile.SetColor(tile.isValid ? tile.defaultColor : Color.black);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
