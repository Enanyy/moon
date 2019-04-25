using UnityEngine;
using  PBMessage;
public class MSG_BattleEntityIdleNotify : Message<BattleEntityIdleNotify>
{
    public MSG_BattleEntityIdleNotify() : base(MessageID.BATTLE_ENTITY_IDLE_NOTIFY)
    {

    }

    protected override void OnMessage()
    {
        //Debug.Log(ret.id+" idle");
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            entity.UpdateEntity(message.data);
            if (entity.machine != null && entity.machine.current != null &&
                entity.machine.current.type == (int)ActionType.Run)
            {
                var action = entity.machine.current as EntityAction;
                if (action != null)
                {
                    action.destination = new Vector3(message.data.position.x, 0, message.data.position.y); ;
                    action.doneWhenSync = true;
                    action.sync = true;
                }
            }
        }
    }
}
