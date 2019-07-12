using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class SphereGridTest : MonoBehaviour {

    public float SphereRadius = 1;
    public int SphereDetail = 0;


    public Material material;

    private SphereGrid grid;

    public int range = 1;

    public SphereEntity entity;

    // Use this for initialization
    void Awake () {

        
        click = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        click.transform.SetParent(transform);
        click.GetComponent<SphereCollider>().enabled = false;

        grid = new SphereGrid();

        string path = Application.dataPath + "/spheregrid.txt";
        if (File.Exists(path))
        {
            string text = File.ReadAllText(path);
            grid.ParseTilesType(text);
        }

        grid.Init(SphereRadius,SphereDetail);
        grid.SetRoot(transform);

        

    }
    private Tile mSelectTile;
    private GameObject click;


    public void OnRenderObject()
    {
        if (grid != null)
        {
            grid.GLDraw();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (grid != null)
            {
                var tile = GetMousePositionTile();

                if (tile != null)
                {   
                    mSelectTile = tile;

                    if(entity!= null)
                    {
                        entity.MoveTo(mSelectTile);
                    }
                }
                else
                {
                    mSelectTile = null;
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (grid != null && mSelectTile != null)
            {

                var tile = GetMousePositionTile();

                if (tile != null)
                {
                    var points = grid.FindPath(mSelectTile,
                        tile,
                        (t) => { return true; }
                    );

                    if (points != null)
                    {
                        while (points.Count > 0)
                        {
                            var p = points.Pop();
                            Debug.Log(p.index);
                        }
                    }
                }
            }

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
}
