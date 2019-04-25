using PBMessage;
public class MSG_BattleEntityBloodNotify : Message<BattleEntityBloodNotify>
{
    public MSG_BattleEntityBloodNotify() : base(MessageID.BATTLE_ENTITY_BLOOD_NOTIFY)
    {

    }

    protected override void OnMessage()
    {
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            entity.DropBlood(message.value);
        }
    }
}
