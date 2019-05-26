using UnityEngine;
using System.Collections;

public class Chess : MonoBehaviour
{
    public int lines = 12;
    public int columns = 16;
    public float tileWidth = 2.5f;
    public float tileHeight = 2.5f;

    public bool showGrid = true;

    public Vector3 original = new Vector3(0, 0, 0);
    // Use this for initialization
    void Start()
    {
        BattleGrid.Instance.Init(original, lines, columns, tileWidth, tileHeight);
    }

    // Update is called once per frame
    void Update()
    {
        BattleGrid.Instance.Update();

        BattleGrid.Instance.showGrid = showGrid;
    }
}
