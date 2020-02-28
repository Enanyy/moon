using System;
using System.Collections.Generic;

public class ActionPluginRemove : ActionPlugin
{
    public override void OnStateExit()
    {
        base.OnStateExit();
        BattleManager.Instance.RemoveEntity(agent.id);
    }
}

