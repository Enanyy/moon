using UnityEngine;
using PBMessage;
public class MSG_BattleEntityIdleNotify : Message<BattleEntityIdleNotify>
{
    public MSG_BattleEntityIdleNotify() : base(MessageID.BATTLE_ENTITY_IDLE_NOTIFY)
    {

    }
    public static MSG_BattleEntityIdleNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleEntityIdleNotify>(MessageID.BATTLE_ENTITY_IDLE_NOTIFY);
    }
    protected override void OnRecv()
    {
        
        //Debug.Log(ret.id+" idle");
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            Vector3 pos = new Vector3(message.position.x, 0, message.position.y);
            if (entity.machine != null && entity.machine.current != null &&
                entity.machine.current.type == (int)ActionType.Run)
            {
                var action = entity.machine.current as EntityAction;
                if (action != null)
                {
                  
                    action.AddPathPoint(pos, Vector3.zero, true);
                }
            }
        }
    }
}
