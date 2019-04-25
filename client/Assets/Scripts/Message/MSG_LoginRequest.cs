
using  PBMessage;
public class MSG_LoginRequest : Message<LoginRequest>
{
    public MSG_LoginRequest() : base(MessageID.LOGIN_REQUEST)
    {

    }

    protected override void OnMessage()
    {
       
    }
}
