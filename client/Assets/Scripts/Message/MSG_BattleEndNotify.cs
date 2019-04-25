using UnityEngine;
using  PBMessage;
public class MSG_BattleEndNotify : Message<BattleEndNotify>
{
    public MSG_BattleEndNotify() : base(MessageID.BATTLE_END_NOTIFY)
    {

    }

    protected override void OnMessage()
    {
        Debug.Log("Battle end:" + message.copy);

        BattleManager.Instance.Destroy();

        NetworkManager.Instance.Close(ConnectID.Game);
    }
}
