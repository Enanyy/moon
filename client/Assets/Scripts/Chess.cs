using UnityEngine;
using System.Collections;

public class Chess : MonoBehaviour
{
    public bool rectGrid = true;
    public int lines = 12;
    public int columns = 16;
    public float tileWidth = 2.5f;
    public float tileHeight = 2.5f;

    public bool showGrid = true;

    public Vector3 original = new Vector3(0, 0, 0);

    public HexGridShape shape = HexGridShape.Hexagon;

    public HexOrientation orientation = HexOrientation.Pointy;
    // Use this for initialization
    void Start()
    {
        if (rectGrid)
        {
            BattleRectGrid.Instance.Init(original, lines, columns, tileWidth, tileHeight);
        }
        else
        {
            BattleHexGrid.Instance.Init(original, shape, lines, columns, Mathf.Max(tileWidth, tileHeight), orientation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rectGrid)
        {
            BattleRectGrid.Instance.Update();

            BattleRectGrid.Instance.showGrid = showGrid;

        }
        else
        {
            BattleHexGrid.Instance.Update();

            BattleHexGrid.Instance.showGrid = showGrid;
        }
        
     
    }
}
