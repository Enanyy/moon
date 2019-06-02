using UnityEngine;


public enum BattleGrid
{
    Rect,
    Hex,
}




public class BattleGridEntity : BattleEntity
{
    public BattleTile tile { get; private set; }

    public TB_Hero hero { get; private set; }

    public int heroid;

    public BattleGrid grid = BattleGrid.Rect;

    public override Vector3 position
    {
        get
        {
            return base.position;
        }

        set
        {
            base.position = value;

            BattleTile t = null;

            if (grid == BattleGrid.Rect)
            {
                t = BattleRectGrid.Instance.TileAt(base.position);               
            }
            else
            {
                t = BattleHexGrid.Instance.TileAt(base.position);
            }
            if (t != null && t != tile)
            {
                if (tile != null)
                {
                    tile.entity = null;
                }
                t.entity = this;
                tile = t;
            }
        }
    }

    public override void Init()
    {

        hero = Table_Hero.Get(heroid);

        if (hero != null)
        {
            configid = (uint)hero.configid;
        }

        base.Init();

    }
   
    public override void Clear()
    {
        base.Clear();

        tile = null;

        hero = null;

        heroid = 0;
    }

    

}