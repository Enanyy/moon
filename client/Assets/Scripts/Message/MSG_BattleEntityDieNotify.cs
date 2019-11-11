using UnityEngine;
using PBMessage;
public class MSG_BattleEntityDieNotify : Message<BattleEntityDieNotify>
{
    public MSG_BattleEntityDieNotify() : base(MessageID.BATTLE_ENTITY_DIE_NOTIFY)
    {

    }
    public static MSG_BattleEntityDieNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleEntityDieNotify>(MessageID.BATTLE_ENTITY_DIE_NOTIFY);
    }
    protected override void OnRecv()
    {
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            //entity.UpdateEntity(ret.data);
            entity.Die();
        }
    }
}
