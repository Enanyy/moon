using UnityEngine;


public class BattleGridEntity : BattleEntity
{
    public ITile tile { get; private set; }

    public TB_Hero hero { get; private set; }

    public int heroid;

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