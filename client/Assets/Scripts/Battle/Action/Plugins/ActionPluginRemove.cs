using System;
using System.Collections.Generic;

public class ActionPluginRemove : ActionPlugin
{
    public override void OnExit()
    {
        base.OnExit();
        BattleManager.Instance.RemoveEntity(agent.id);
    }
}

