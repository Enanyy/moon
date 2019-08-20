#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SphereGridEditor : MonoBehaviour {

    public float SphereRadius = 1;
    public int SphereDetail = 6;

    private SphereGrid grid;
    private GameObject click;

    private SphereRotate rotate;

    private bool brushing = false;

    public int brushSize = 2;

    private Vector3 prevMousePosition;

    public TileType type = TileType.Free;

   
   
    // Use this for initialization
    void Awake()
    {
        rotate = GetComponent<SphereRotate>();
        if (rotate == null) rotate = gameObject.AddComponent<SphereRotate>();
        
        click = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        click.GetComponent<SphereCollider>().enabled = false;

        grid = new SphereGrid();

        grid.Init(SphereRadius, SphereDetail);
        grid.SetRoot(transform);

        string path = Application.dataPath + "/spheregrid.txt";
        if (File.Exists(path))
        {
            string text = File.ReadAllText(path);
            grid.ParseTilesType(text);
        }

        for(int i = 0; i < grid.tiles.Count; ++i)
        {
            grid.tiles[i].SetDefaultColor();
        }
    }
   

    public void OnRenderObject()
    {
        if (grid != null)
        {
            grid.GLDraw();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            brushing = true;
            rotate.enabled = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            brushing = false;
            rotate.enabled = true;
        }

        if (brushing)
        {
            if (prevMousePosition != Input.mousePosition)
            {
                Brush();
            }

            prevMousePosition = Input.mousePosition;
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 40), "Clear"))
        {
            grid.tilesType.Clear();
         
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


                var tiles = grid.TilesInRange(point, r);

                click.transform.position = point;

                return tiles;
            }

        }

        return null;
    }



    [ContextMenu("Save")]
    void Save()
    {
        if (grid != null)
        {
            string text = grid.FormatTilesType();
            string path = Application.dataPath + "/spheregrid.txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, text);
        }
    }
    [ContextMenu("Save as XML")]
    void SaveAsXml()
    {
        if (grid != null)
        {
            string text = grid.ToXml();

            string path = Application.dataPath + "/spheregrid.xml";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, text);
        }
    }

    void Brush()
    {
        var tiles = GetMousePositionTiles(brushSize);
        if (tiles != null)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                if (type == TileType.Discard)
                {
                    if (grid.tilesType.ContainsKey(tile.index))
                    {
                        grid.tilesType.Remove(tile.index);
                    }
                }
                else
                {

                    if (grid.tilesType.ContainsKey(tile.index) == false)
                    {
                        grid.tilesType.Add(tile.index, type);
                    }
                    else
                    {
                        grid.tilesType[tile.index] = type;
                    }
                }
                tile.SetDefaultColor();
               
            }
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

   
}
#endif