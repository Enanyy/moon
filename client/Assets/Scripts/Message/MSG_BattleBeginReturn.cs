using UnityEngine;
using PBMessage;
public class MSG_BattleBeginReturn : Message<BattleBeginReturn>
{
    public MSG_BattleBeginReturn() : base(MessageID.BATTLE_BEGIN_RETURN)
    {

    }
    public static MSG_BattleBeginReturn Get()
    {
        return MessageManager.Instance.Get<MSG_BattleBeginReturn>(MessageID.BATTLE_BEGIN_RETURN);
    }
    protected override void OnMessage()
    {
       
    }
}
