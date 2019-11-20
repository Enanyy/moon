using UnityEngine;
using PBMessage;
public class MSG_LoginRequest : Message<LoginRequest>
{
    public MSG_LoginRequest() : base(MessageID.LOGIN_REQUEST)
    {

    }
    public static MSG_LoginRequest Get()
    {
        return MessageManager.Instance.Get<MSG_LoginRequest>(MessageID.LOGIN_REQUEST);
    }
    protected override void OnRecv(Connection connection)
    {
       
    }
}
