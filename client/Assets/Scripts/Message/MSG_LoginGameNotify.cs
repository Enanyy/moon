using UnityEngine;
using PBMessage;
public class MSG_LoginGameNotify : Message<LoginGameNotify>
{
    public MSG_LoginGameNotify() : base(MessageID.LOGIN_GAME_NOTIFY)
    {

    }
    public static MSG_LoginGameNotify Get()
    {
        return MessageManager.Instance.Get<MSG_LoginGameNotify>(MessageID.LOGIN_GAME_NOTIFY);
    }
    protected override void OnRecv(Connection connection)
    {
       
        Debug.Log(message.ip + ":" + message.port);

        NetworkManager.Instance.Connect((int)ConnectID.Game, message.ip, message.port, (c) =>
        {
            Debug.Log("Connect Game Success");

            if ( Main.Instance.user != null)
            {
                MSG_LoginGameRequest request = MSG_LoginGameRequest.Get();
                request.message.id = Main.Instance.user.id;
                request.Send(ConnectID.Game);
            }

        }, (c) =>
        {
            Debug.Log("Connect Game Fail");
            NetworkManager.Instance.Close(c.ID);
        });
    }
}
