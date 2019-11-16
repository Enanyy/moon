using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class SphereGridTest : MonoBehaviour {

    public float SphereRadius = 1;
    public int SphereDetail = 0;


    public Material material;

    public SphereGrid grid;

    public int range = 1;

    public SphereEntity entity;

    // Use this for initialization
    void Awake () {

        mainCamera = Camera.main;

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
#if UNITY_EDITOR
        for (int i = 0; i < grid.tiles.Count; ++i)
        {
            grid.tiles[i].SetDefaultColor();
        }
#endif

    }
    private Tile mSelectTile;
    private GameObject click;

#if UNITY_EDITOR
    public void OnRenderObject()
    {
        if (grid != null)
        {
            grid.GLDraw();
        }
    }
#endif
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

                    entity.MoveTo(mSelectTile);
                }
                else
                {
                    mSelectTile = null;
                }
            }
        }
        Scroll();
    }

    public  Tile GetMousePositionTile()
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



    public Camera mainCamera { get; private set; }

  
    //缩放距离限制   

    private float minScrollDistance = 120;
    private float maxScrollDistance = 500;
    private float scrollSpeed = 20;
    void Scroll()
    {
        // 鼠标滚轮触发缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if ((scroll < -0.001 || scroll > 0.001) && mainCamera)
        {
            float displacement = scrollSpeed * scroll;

            mainCamera.transform.position += mainCamera.transform.forward * displacement;
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            float distance = 0;

            Plane plane = new Plane(mainCamera.transform.position - grid.root.position, grid.root.position);

            plane.Raycast(ray, out distance);

            if (distance < minScrollDistance)
            {
                mainCamera.transform.position = ray.GetPoint(distance - minScrollDistance);
            }
            else if (distance > maxScrollDistance)
            {
                mainCamera.transform.position = ray.GetPoint(distance - maxScrollDistance);
            }
        }

    }
}
