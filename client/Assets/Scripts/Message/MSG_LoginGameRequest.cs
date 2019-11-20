using UnityEngine;
using PBMessage;
public class MSG_LoginGameRequest : Message<LoginGameRequest>
{
    public MSG_LoginGameRequest() : base(MessageID.LOGIN_GAME_REQUEST)
    {

    }

    public static MSG_LoginGameRequest Get()
    {
        return MessageManager.Instance.Get<MSG_LoginGameRequest>(MessageID.LOGIN_GAME_REQUEST);
    }

    protected override void OnRecv(Connection connection)
    {
       
    }
}
