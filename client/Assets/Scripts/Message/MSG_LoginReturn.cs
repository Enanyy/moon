using UnityEngine;
using PBMessage;
public class MSG_LoginReturn : Message<LoginReturn>
{
    public MSG_LoginReturn() : base(MessageID.LOGIN_RETURN)
    {

    }
    public static MSG_LoginReturn Get()
    {
        return MessageManager.Instance.Get<MSG_LoginReturn>(MessageID.LOGIN_RETURN);
    }
    protected override void OnRecv(Connection connection)
    {
      
        UnityEngine. Debug.Log("Login result:" + message.result);

       Main.Instance. user = message.userdata;
    }
}
