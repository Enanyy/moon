using PBMessage;
public class MSG_LoginReturn : Message<LoginReturn>
{
    public MSG_LoginReturn() : base(MessageID.LOGIN_RETURN)
    {

    }

    protected override void OnMessage()
    {
      
        UnityEngine. Debug.Log("Login result:" + message.result);

       Main.Instance. user = message.userdata;
    }
}