using UnityEngine;
using System.Collections;

public class Chess : MonoBehaviour
{
    public BattleGrid grid;
    public int lines = 12;
    public int columns = 16;
    public float tileWidth = 2.5f;
    public float tileHeight = 2.5f;

    public bool showGrid = true;

    public Vector3 original = new Vector3(0, 0, 0);

    public HexGridShape shape = HexGridShape.Hexagon;

    public HexOrientation orientation = HexOrientation.Pointy;

    public bool jump = true;

    public static Chess Instance { get; private set; }
    // Use this for initialization
    void Start()
    {
        Instance = this;

        CameraManager.Instance.Init();
        TextAsset data = Resources.Load<TextAsset>("r/database/data");
        if (data != null)
        {
            DataTableManager.Instance.Init(data.bytes);
        }
        if (grid == BattleGrid.Rect)
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
        if (grid == BattleGrid.Rect)
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
