using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
        grid.Init(SphereRadius,SphereDetail);
        grid.SetRoot(transform);
   
        grid.Show(material);
       

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

    void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            if (grid != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Default")))
                {
                    Vector3 point = hit.point;

                    //var tiles = grid.TilesInRange(point, range);
                    //for (int i = 0; i < tiles.Count; i++)
                    //{
                    //    tiles[i].SetColor(Color.green);
                    //}

                    var tile = grid.TileAt(point);

                    click.transform.position = point;

                    if (tile != null)
                    {
                        Debug.Log(tile.index);
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
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (grid != null && mSelectTile != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Default")))
                {
                    Vector3 point = hit.point;

                    var tile = grid.TileAt(point);

                    click.transform.position = point;

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

        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (mSelectTile != null)
            {
                mSelectTile.isValid = !mSelectTile.isValid;
                mSelectTile.SetColor(mSelectTile.isValid? mSelectTile.defaultColor:Color.black);
            }
        }
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
            tile.SetColor(color);
        }
    }

    [ContextMenu("Save")]
    void Save()
    {
        if (grid != null)
        {
            string xml = grid.ToXml();

            string path = Application.dataPath + "/grid.xml";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path,xml);
        }
    }

    

}
