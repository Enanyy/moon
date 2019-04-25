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
                LoginGameRequest request = new LoginGameRequest();
                request.id = Main.Instance.user.id;
                NetworkManager.Instance.Send(ConnectID.Game, MessageID.LOGIN_GAME_REQUEST, request);
            }

        }, (c) =>
        {
            UnityEngine.Debug.Log("Connect Game Fail");
            NetworkManager.Instance.Close(c.ID);
        });
    }
}
