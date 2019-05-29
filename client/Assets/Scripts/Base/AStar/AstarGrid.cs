using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// 单独的格子
/// </summary>
public class Cell
{
    public int x;
    public int y;
    public bool isWall;
    // 与起点的长度
    public int gCost;
    // 与目标点的长度
    public int hCost;
    // 总的路径长度
    public int fCost
    {
        get { return gCost + hCost; }
    }

    // 父节点
    public Cell parent = null;
    public GameObject obj;

    public Cell(int x, int y, bool isWall, GameObject obj)
    {
        this.x = x;
        this.y = y;
        this.isWall = isWall;
        this.obj = obj;
    }

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }
    public string PrintCost()
    {
        return "{ fCost:" + fCost + ",hCost:" + hCost + "}";
    }

}

/// <summary>
/// 地面的格子构建
/// 这里只是简单的demo，没有优化也没有调整，只是纯展示A*算法
/// </summary>
public class AstarGrid : MonoBehaviour
{

    public GameObject CellAsset;
    public GameObject PointAsset;

    public int gridSize = 10;
    int cellSize = 50;

    List<string> wallList = new List<string>() { "5_5", "5_6", "5_7" };
    public List<Cell> cells = new List<Cell>();

    public Vector2 startPos = new Vector2(3, 6);
    public Vector2 endPos = new Vector2(7, 6);

    // Use this for initialization
    void Awake()
    {
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateGrid()
    {

        //创建格子；
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {

                GameObject obj = GameObject.Instantiate<GameObject>(CellAsset, transform);
                RectTransform trans = obj.transform as RectTransform;
                trans.anchoredPosition = new Vector2(i, j) * cellSize;
                bool isWall = false;
                trans.Find("Text").GetComponent<Text>().text = i + "," + j;
                if (wallList.Contains(i + "_" + j))
                {
                    isWall = true;

                    obj.GetComponent<Image>().color = Color.black;

                }
                Cell cell = new Cell(i, j, isWall, obj);
                cells.Add(cell);
            }
        }

        //创建起点
        GameObject obj1 = GameObject.Instantiate<GameObject>(PointAsset, transform);
        RectTransform trans1 = obj1.transform as RectTransform;
        trans1.anchoredPosition = startPos * cellSize;
        obj1.GetComponent<Image>().color = Color.red;
        trans1.localScale = Vector3.one * 1.2f;
        obj1.name = "start";

        //创建终点
        GameObject obj2 = GameObject.Instantiate<GameObject>(PointAsset, transform);
        RectTransform trans2 = obj2.transform as RectTransform;
        trans2.anchoredPosition = endPos * cellSize;
        obj2.GetComponent<Image>().color = Color.green;
        trans2.localScale = Vector3.one * 1.2f;
        obj2.name = "end";
    }


    public Cell GetCell(Vector2 pos)
    {
        return cells.Find((c) => { return c.x == (int)pos.x && c.y == (int)pos.y; });
    }


    public void CreatePath(Cell endPos)
    {
        Cell cell = endPos;
        while (cell != null)
        {
            GameObject obj1 = GameObject.Instantiate<GameObject>(PointAsset, transform);
            RectTransform trans1 = obj1.transform as RectTransform;
            trans1.anchoredPosition = cellSize * new Vector2(cell.x, cell.y);
            obj1.GetComponent<Image>().color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.5f);
            cell = cell.parent;
        }

    }

}
