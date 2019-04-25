using PBMessage;
public class MSG_LoginGameRequest : Message<LoginGameRequest>
{
    public MSG_LoginGameRequest() : base(MessageID.LOGIN_GAME_REQUEST)
    {

    }

    protected override void OnMessage()
    {
       
    }
}
