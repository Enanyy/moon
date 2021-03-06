using UnityEngine;
using PBMessage;
[MessageHandler]
public class MSG_BattleEntityBloodNotify : Message<BattleEntityBloodNotify>
{
    public MSG_BattleEntityBloodNotify() : base(MessageID.BATTLE_ENTITY_BLOOD_NOTIFY)
    {

    }
    public static MSG_BattleEntityBloodNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleEntityBloodNotify>(MessageID.BATTLE_ENTITY_BLOOD_NOTIFY);
    }
    protected override void OnRecv(Connection connection)
    {
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            entity.DropBlood(message.value);
        }
    }
}
