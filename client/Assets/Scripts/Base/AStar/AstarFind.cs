using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// A*算法寻路
/// </summary>
public class AstarFind : MonoBehaviour
{


    AstarGrid grid;
    void Start()
    {
        grid = GetComponent<AstarGrid>();
        FindPath(grid.startPos, grid.endPos);
    }





    /// <summary>
    /// 使用A*算法寻路
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    void FindPath(Vector2 start, Vector2 end)
    {
        //和A*算法无关，只是为了显示使用
        int showFindNum = 1;

        //开启列表
        List<Cell> openLs = new List<Cell>();
        //关闭列表
        List<Cell> closeLs = new List<Cell>();

        //起点
        Cell startCell = grid.GetCell(start);
        //终点
        Cell endCell = grid.GetCell(end);
        Debug.LogFormat("寻路开始,start({0}),end({1})!", start, end);

        //将起点作为待处理的点放入开启列表，
        openLs.Add(startCell);

        //如果开启列表没有待处理点表示寻路失败，此路不通
        while (openLs.Count > 0)
        {
            //遍历开启列表，找到消费最小的点作为检查点
            Cell cur = openLs[0];
            for (int i = 0; i < openLs.Count; i++)
            {
                if (openLs[i].fCost < cur.fCost && openLs[i].hCost < cur.hCost)
                {
                    cur = openLs[i];
                }
            }
            Debug.Log("当前检查点：" + cur.ToString() + " 编号：" + showFindNum + "  open列表节点数量：" + openLs.Count);
            //显示在界面，和A*算法无关
            cur.obj.transform.Find("Num").GetComponent<Text>().text = showFindNum.ToString();
            showFindNum++;

            //从开启列表中删除检查点，把它加入到一个“关闭列表”，列表中保存所有不需要再次检查的方格。
            openLs.Remove(cur);
            closeLs.Add(cur);

            //检查是否找到终点
            if (cur == endCell)
            {
                Debug.Log("寻路结束!");
                grid.CreatePath(cur);
                return;
            }

            //根据检查点来找到周围可行走的点
            //1.如果是墙或者在关闭列表中则跳过
            //2.如果点不在开启列表中则添加
            //3.如果点在开启列表中且当前的总花费比之前的总花费小，则更新该点信息
            List<Cell> aroundCells = GetAllAroundCells(cur);
            foreach (var cell in aroundCells)
            {

                if (cell.isWall || closeLs.Contains(cell))
                    continue;

                int cost = cur.gCost + GetDistanceCost(cell, cur);

                if (cost < cell.gCost || !openLs.Contains(cell))
                {
                    cell.gCost = cost;
                    cell.hCost = GetDistanceCost(cell, endCell);
                    cell.parent = cur;
                    Debug.Log("cell:" + cell.ToString() + "  parent:" + cur.ToString() + "  " + cell.PrintCost());
                    if (!openLs.Contains(cell))
                    {
                        openLs.Add(cell);
                    }

                    //显示在界面，和A*算法无关
                    cell.obj.transform.Find("Cost").GetComponent<Text>().text = cell.fCost.ToString();
                }


            }

        }

        Debug.Log("寻路失败!");
    }


    // 取得周围的节点
    public List<Cell> GetAllAroundCells(Cell cell)
    {
        List<Cell> list = new List<Cell>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // 如果是自己，则跳过
                if (i == 0 && j == 0)
                    continue;
                int x = cell.x + i;
                int y = cell.y + j;
                // 判断是否越界，如果没有，加到列表中
                if (x < grid.gridSize && x >= 0 && y < grid.gridSize && y >= 0)
                {
                    list.Add(grid.GetCell(new Vector2(x, y)));
                }

            }
        }
        return list;
    }


    /// <summary>
    /// 估价函数，启发函数
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    int GetDistanceCost(Cell a, Cell b)
    {
        int cntX = Mathf.Abs(a.x - b.x);
        int cntY = Mathf.Abs(a.y - b.y);
        // 判断到底是那个轴相差的距离更远
        if (cntX > cntY)
        {
            return 14 * cntY + 10 * (cntX - cntY);
        }
        else
        {
            return 14 * cntX + 10 * (cntY - cntX);
        }
    }

}
