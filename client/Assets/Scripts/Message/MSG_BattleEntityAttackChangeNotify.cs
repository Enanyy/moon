using UnityEngine;
using PBMessage;
public class MSG_BattleEntityAttackChangeNotify : Message<BattleEntityAttackChangeNotify>
{
    public MSG_BattleEntityAttackChangeNotify() : base(MessageID.BATTLE_ENTITY_ATTACK_CHANGE_NOTIFY)
    {

    }

    public static MSG_BattleEntityAttackChangeNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleEntityAttackChangeNotify>(MessageID.BATTLE_ENTITY_ATTACK_CHANGE_NOTIFY);
    }

    protected override void OnMessage()
    {
        BattleEntity entity = BattleManager.Instance.GetEntity(message.id);
        if(entity!= null)
        {
            if(entity.machine!= null && entity.machine.current!= null && entity.machine.current.type == (int)ActionType.Attack)
            {
                var attack = entity.machine.current as EntityAction;
                attack.duration = message.duration;
                attack.speed = message.speed;
            }
        }
   }
}
