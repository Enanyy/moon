using UnityEngine;


public enum BattleGrid
{
    Rect,
    Hex,
}




public class BattleGridEntity : BattleEntity
{
    public BattleTile tile { get; private set; }

    public TBHero hero { get; private set; }

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

        hero = DTHero.Get(heroid);

        if (hero != null)
        {
            config = hero.config;
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