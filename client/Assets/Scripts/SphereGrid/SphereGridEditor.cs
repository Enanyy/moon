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

    private bool onDrag = false;  //是否被拖拽//    
    public float speed = 6f;   //旋转速度//    
    private float currentSpeed;   //阻尼速度// 
    private float axisX = 1;
    //鼠标沿水平方向移动的增量//   
    private float axisY = 1;    //鼠标沿竖直方向移动的增量//   

    private float currentVelocity = 0;
    private float smoothTime = 1;

    private bool controll = false;

    public int brushSize = 2;


    public TileType type = TileType.Free;

    private  Dictionary<int,TileType> mTileDic = new Dictionary<int, TileType>();

    private Dictionary<TileType, Color> mTileColorDic = new Dictionary<TileType, Color>
    {
        {TileType.Black, Color.gray},
        {TileType.Free, Color.green},
    };
    // Use this for initialization
    void Awake()
    {
        
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
                string[] str = indexs[i].Split(':');
                if (str.Length == 2)
                {
                    int index = str[0].ToInt32Ex();
                    if (mTileDic.ContainsKey(index) == false)
                    {
                        mTileDic.Add(index, (TileType)str[1].ToInt32Ex());
                    }
                }
            }
        }

        grid.Init(SphereRadius, SphereDetail);
        grid.SetRoot(transform);


        for (int i = 0; i < grid.tiles.Count; ++i)
        {
            var tile = grid.tiles[i];
            Color color = Color.gray;
            if (mTileDic.ContainsKey(tile.index))
            {
                var type = mTileDic[tile.index];
                if (mTileColorDic.ContainsKey(type))
                {
                    color = mTileColorDic[type];
                }
            }

            tile.color = color;
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
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            controll = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            controll = false;
        }

        if (controll == false)
        {
            if (onDrag == false)
            {
                if (currentSpeed > 0.01f)
                {
                    currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref currentVelocity, smoothTime);
                }
                else
                {
                    currentSpeed = 0;
                }
                if (currentSpeed < 0) currentSpeed = 0;
            }
            else
            {
                currentSpeed = speed;
            }

            if (currentSpeed != 0 && controll == false)
            {
                transform.Rotate(new Vector3(axisY, axisX, 0) * currentSpeed, Space.World);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Brush();
            }
        }

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 40), "Clear"))
        {
            mTileDic.Clear();
            for (int i = 0; i < grid.tiles.Count; ++i)
            {
                var tile = grid.tiles[i];
                tile.color = Color.gray;
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
            StringBuilder builder = new StringBuilder();
            var it = mTileDic.GetEnumerator();
            while (it.MoveNext())
            {
                builder.AppendFormat("{0}:{1};", it.Current.Key, (int) it.Current.Value);
            }

            string path = Application.dataPath + "/grid.txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, builder.ToString());
        }
    }


  
    void OnMouseDown()
    {
        //接受鼠标按下的事件// 
        axisX = 0f; axisY = 0f;
    }

    void OnMouseUp()
    {
        onDrag = false;
        currentSpeed = speed;
    }
    void OnMouseDrag()     //鼠标拖拽时的操作// 
    {
        if (controll)
        {
            Brush();
        }
        else
        {
            onDrag = true;
            axisX = -Input.GetAxis("Mouse X");
            //获得鼠标增量// 
            axisY = Input.GetAxis("Mouse Y");
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
                if (type == TileType.Black)
                {
                    if (mTileDic.ContainsKey(tile.index))
                    {
                        mTileDic.Remove(tile.index);
                    }
                }
                else
                {

                    if (mTileDic.ContainsKey(tile.index) == false)
                    {
                        mTileDic.Add(tile.index, type);
                    }
                    else
                    {
                        mTileDic[tile.index] = type;
                    }
                }
                tile.color = mTileColorDic[type];
            }
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

   
}
#endif