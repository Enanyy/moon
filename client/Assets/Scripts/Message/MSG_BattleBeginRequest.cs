using UnityEngine;
using PBMessage;
public class MSG_BattleBeginRequest : Message<BattleBeginRequest>
{
    public MSG_BattleBeginRequest() : base(MessageID.BATTLE_BEGIN_REQUEST)
    {

    }

    public static MSG_BattleBeginRequest Get()
    {
        return MessageManager.Instance.Get<MSG_BattleBeginRequest>(MessageID.BATTLE_BEGIN_REQUEST);
    }

    protected override void OnRecv(Connection connection)
    {
       
    }
}
