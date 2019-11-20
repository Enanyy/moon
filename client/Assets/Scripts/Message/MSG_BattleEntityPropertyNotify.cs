using UnityEngine;
using PBMessage;
public class MSG_BattleEntityPropertyNotify : Message<BattleEntityPropertyNotify>
{
    public MSG_BattleEntityPropertyNotify() : base(MessageID.BATTLE_ENTITY_PROPERTY_NOTIFY)
    {

    }

    public static MSG_BattleEntityPropertyNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleEntityPropertyNotify>(MessageID.BATTLE_ENTITY_PROPERTY_NOTIFY);
    }

    protected override void OnRecv(Connection connection)
    {
       
    }
}
