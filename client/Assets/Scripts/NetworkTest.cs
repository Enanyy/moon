using System;
using System.Collections.Generic;
using UnityEngine;
using PBMessage;

public class NetworkTest : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = 12345;
    public string username = "lwg";
    public string pwd = "";

    void Start()
    {
        Debug.Log("IsLittleEndian:"+BitConverter.IsLittleEndian);
        NetworkManager.Instance().onReceive += OnReceive;
    }

    void LateUpdate()
    {
        NetworkManager.Instance().Update();
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 100, 40), "Connect"))
        {
            NetworkManager.Instance().Connect(ConnectID.Logic, ip, port,
                delegate(Connection connection)
                {
                    Debug.Log("Connect");
                },
                delegate(Connection connection)
                {
                    Debug.Log("Disconnect");
                });
        }

        if (GUI.Button(new Rect(20, 80, 100, 40), "Login"))
        {
            LoginRequest request = new LoginRequest();
            request.name = username;
            request.password = pwd;


           NetworkManager.Instance().Send(ConnectID.Logic,MessageID.LOGIN_REQUEST,request);
        }

        if (user !=null&&GUI.Button(new Rect(20, 140, 100, 40), "Battle"))
        {
            BattleBeginRequest request = new BattleBeginRequest();
            NetworkManager.Instance().Send(ConnectID.Logic, MessageID.BATTLE_BEGIN_REQUEST, request);
        }
    }

    private UserData user = null;

    void OnReceive(NetPacket packet)
    {
        Debug.Log(packet.ID);
        var id = (MessageID) packet.ID;
        switch (id)
        {
            case MessageID.LOGIN_RETURN:
            {
                LoginReturn ret = ProtoTransfer.DeserializeProtoBuf<LoginReturn>(packet.data,
                    NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);
                Debug.Log("Login result:" + ret.result);

                user = ret.userdata;
            }
                break;
            case MessageID.LOGIN_GAME_NOTIFY:
            {
                LoginGameNotify ret = ProtoTransfer.DeserializeProtoBuf<LoginGameNotify>(packet.data,
                    NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);
                Debug.Log(ret.ip + ":" + ret.port);

                NetworkManager.Instance().Connect(ConnectID.Game, ret.ip, ret.port, (c) =>
                {
                    Debug.Log("Connect Game Success");

                    if (user != null)
                    {
                        LoginGameRequest request = new LoginGameRequest();
                        request.id = user.id;
                        NetworkManager.Instance().Send(ConnectID.Game, MessageID.LOGIN_GAME_REQUEST, request);
                    }

                }, (c) => { Debug.Log("Connect Game Fail"); });

            }
                break;
            case MessageID.LOGIN_GAME_RETURN:
            {
                LoginGameReturn ret = ProtoTransfer.DeserializeProtoBuf<LoginGameReturn>(packet.data,
                    NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);

            }
                break;
            case MessageID.BATTLE_BEGIN_RETURN:
            {
                BattleBeginReturn ret = ProtoTransfer.DeserializeProtoBuf<BattleBeginReturn>(packet.data,
                    NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);

                Debug.Log("Battle begin result:" + ret.result);
            }
                break;
            case MessageID.BATTLE_BEGIN_NOTIFY:
            {
                BattleBeginNotify ret = ProtoTransfer.DeserializeProtoBuf<BattleBeginNotify>(packet.data,
                    NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);
                Debug.Log("Battle begin:"+ret.copy);
            }
                break;
            case MessageID.BATTLE_END_NOTIFY:
            {
                    BattleEndNotify ret = ProtoTransfer.DeserializeProtoBuf<BattleEndNotify>(packet.data,
                        NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);
                    Debug.Log("Battle end:" + ret.copy);
                }
                break;
        }
    }


    void OnApplicationQuit()
    {
        NetworkManager.Instance().Close();
    }
}


