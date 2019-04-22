using System;
using System.Collections.Generic;

public class ActionRemovePlugin : ActionPlugin
{
    public override void OnExit()
    {
        base.OnExit();
        BattleManager.Instance.RemoveEntity(agent.id);
    }
}

