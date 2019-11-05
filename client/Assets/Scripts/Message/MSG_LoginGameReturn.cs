using UnityEngine;
using PBMessage;
public class MSG_LoginGameReturn : Message<LoginGameReturn>
{
    public MSG_LoginGameReturn() : base(MessageID.LOGIN_GAME_RETURN)
    {

    }
    public static MSG_LoginGameReturn Get()
    {
        return MessageManager.Instance.Get<MSG_LoginGameReturn>(MessageID.LOGIN_GAME_RETURN);
    }
    protected override void OnRecv()
    {
       
    }
}
