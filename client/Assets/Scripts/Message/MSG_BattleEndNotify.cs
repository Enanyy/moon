using UnityEngine;
using PBMessage;
public class MSG_BattleEndNotify : Message<BattleEndNotify>
{
    public MSG_BattleEndNotify() : base(MessageID.BATTLE_END_NOTIFY)
    {

    }
    public static MSG_BattleEndNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleEndNotify>(MessageID.BATTLE_END_NOTIFY);
    }
    protected override void OnRecv(Connection connection)
    {
        Debug.Log("Battle end:" + message.copy);

        BattleManager.Instance.Destroy();

        NetworkManager.Instance.Close((int)ConnectID.Game);
    }
}
