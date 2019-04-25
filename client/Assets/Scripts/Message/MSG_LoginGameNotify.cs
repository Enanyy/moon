using PBMessage;
public class MSG_LoginGameNotify : Message<LoginGameNotify>
{
    public MSG_LoginGameNotify() : base(MessageID.LOGIN_GAME_NOTIFY)
    {

    }

    protected override void OnMessage()
    {
       
        UnityEngine.Debug.Log(message.ip + ":" + message.port);

        NetworkManager.Instance.Connect(ConnectID.Game, message.ip, message.port, (c) =>
        {
            UnityEngine.Debug.Log("Connect Game Success");

            if ( Main.Instance.user != null)
            {
                MSG_LoginGameRequest request = MessageManager.Instance.Get<MSG_LoginGameRequest>(MessageID.LOGIN_GAME_REQUEST);
                request.message.id = Main.Instance.user.id;
                request.Send(ConnectID.Game);
            }

        }, (c) =>
        {
            UnityEngine.Debug.Log("Connect Game Fail");
            NetworkManager.Instance.Close(c.ID);
        });
    }
}
