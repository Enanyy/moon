using System;
using System.Collections.Generic;
using UnityEngine;
using PBMessage;

public class NetworkTest : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = 5000;
    public string username = "lwg";
    public string pwd = "1234";

    void Start()
    {
        Debug.Log("IsLittleEndian:"+BitConverter.IsLittleEndian);
        NetworkManager.Instance().onReceive += OnReceive;
    }

    void LateUpdate()
    {
        NetworkManager.Instance().Update();
    }

    void Update()
    {
        BattleManager.Instance.Update(Time.deltaTime);

        Drag();
        Scroll();
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
       
        var id = (MessageID) packet.ID;
        //Debug.Log("recv:"+id.ToString());
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
                    //Debug.Log("Login Game result:"+ret.result);
                }
                break;
            case MessageID.BATTLE_BEGIN_RETURN:
                {
                    BattleBeginReturn ret = ProtoTransfer.DeserializeProtoBuf<BattleBeginReturn>(packet.data,
                        NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);

                    //Debug.Log("Battle begin result:" + ret.result);
                }
                break;
            case MessageID.BATTLE_BEGIN_NOTIFY:
                {
                    BattleBeginNotify ret = ProtoTransfer.DeserializeProtoBuf<BattleBeginNotify>(packet.data,
                        NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);
                    //Debug.Log("Battle begin:" + ret.copy);

                    for (int i = 0; i < ret.list.Count; ++i)
                    {
                        var data = ret.list[i];
                        BattleEntity entity = ObjectPool.GetInstance<BattleEntity>();

                        entity.id = data.id;

                        entity.configid = 10000 + data.config;
                        entity.campflag = data.camp;

                        entity.type = data.type;

                        entity.hp = data.data.hp;

                        entity.position = new Vector3(data.data.position.x, 0, data.data.position.y);
                        entity.rotation = Quaternion.LookRotation(new Vector3(data.data.direction.x, 0, data.data.direction.y));
                        if (BattleManager.Instance.AddEntity(entity))
                        {
                            entity.active = true;
                        }
                    }
                }
                break;
            case MessageID.BATTLE_ENTITY_IDLE:
                {
                    BattleEntityIdle ret = ProtoTransfer.DeserializeProtoBuf<BattleEntityIdle>(packet.data,
                        NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);

                    //Debug.Log(ret.id+" idle");
                    var entity = BattleManager.Instance.GetEntity(ret.id);
                    if (entity != null)
                    {
                        if (entity.machine != null && entity.machine.current != null &&
                            entity.machine.current.type == (int)ActionType.Run)
                        {
                            var action = entity.machine.current as EntityAction;
                            if (action != null && action.paths.Count == 0)
                            {
                                action.Done();
                            }
                        }
                    }
                }
                break;
            case MessageID.BATTLE_ENTITY_RUN:
                {
                    BattleEntityRun ret = ProtoTransfer.DeserializeProtoBuf<BattleEntityRun>(packet.data,
                        NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);

                    //Debug.Log(ret.id + " run");

                    Vector3 velocity = new Vector3(ret.velocity.x, 0 ,ret.velocity.y);
                    var entity = BattleManager.Instance.GetEntity(ret.id);
                    if (entity != null)
                    {
                        EntityAction action = entity.GetFirst(ActionType.Run);

                        if (action != null)
                        {
                            action.paths.Clear();
                            action.paths.AddLast(new Vector3(ret.data.position.x, 0, ret.data.position.y));
                            action.velocity = velocity;
                        }
                        else
                        {
                            action = ObjectPool.GetInstance<EntityAction>();
                            action.velocity = velocity;
                            entity.PlayAction(ActionType.Run, action);
                        }
                    }
                }
                break;
            case MessageID.BATTLE_ENTITY_ATTACK:
                {
                    BattleEntityAttack ret = ProtoTransfer.DeserializeProtoBuf<BattleEntityAttack>(packet.data,
                        NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);
                    //Debug.Log(ret.id + " attack");
                    var entity = BattleManager.Instance.GetEntity(ret.id);
                    if (entity != null)
                    {
                        if (entity.machine != null && entity.machine.current != null &&
                            entity.machine.current.type == (int)ActionType.Run)
                        {
                            var run = entity.machine.current as EntityAction;
                            if (run != null && run.paths.Count == 0)
                            {
                                run.Done();
                            }
                        }

                        EntityAction action = ObjectPool.GetInstance<EntityAction>();
                        action.skillid = ret.skill;
                        action.duration = ret.attackspeed;
                        action.target = ret.target;


                        entity.PlayAction(ActionType.Attack, action);
                    }
                }
                break;
            case MessageID.BATTLE_ENTITY_DIE:
                {
                    BattleEntityDie ret = ProtoTransfer.DeserializeProtoBuf<BattleEntityDie>(packet.data,
                        NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);
                    Debug.Log(ret.id + " die");
                    var entity = BattleManager.Instance.GetEntity(ret.id);
                    if (entity != null)
                    {
                        entity.Die();
                    }
                }
                break;
            case MessageID.BATTLE_ENTITY_BLOOD:
                {
                    BattleEntityBlood ret = ProtoTransfer.DeserializeProtoBuf<BattleEntityBlood>(packet.data,
                        NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);

                    var entity = BattleManager.Instance.GetEntity(ret.id);
                    if (entity != null)
                    {
                        entity.DropBlood(ret.value);
                    }
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

    Vector3 oldMousePosition;
    Plane mPlane = new Plane(Vector3.up, Vector3.zero);
    void Drag()
    {
        if (Input.GetMouseButton(1))
        {
            float xDelta = Input.GetAxis("Mouse X");
            float yDelta = Input.GetAxis("Mouse Y");
            if (xDelta != 0.0f || yDelta != 0.0f)
            {
                if (oldMousePosition != Vector3.zero)
                {
                    Ray rayDest = Camera.main.ScreenPointToRay(Input.mousePosition);

                    float distance = 0;
                    mPlane.Raycast(rayDest, out distance);

                    Vector3 dest = rayDest.GetPoint(distance);
                    distance = 0;
                    Ray rayOld = Camera.main.ScreenPointToRay(oldMousePosition);
                    mPlane.Raycast(rayOld, out distance);


                    Vector3 pos = Camera.main.transform.localPosition + rayOld.GetPoint(distance) - dest;

                    Camera.main.transform.localPosition = pos;
                }

                oldMousePosition = Input.mousePosition;
            }

        }
        if (Input.GetMouseButtonUp(1))
        {
            oldMousePosition = Vector3.zero;
        }

    }
    //缩放距离限制   

    private float minScrollDistance = 1;
    private float maxScrollDistance = 100;
    private float scrollSpeed = 20;
    void Scroll()
    {
        // 鼠标滚轮触发缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < -0.001 || scroll > 0.001)
        {
            float displacement = scrollSpeed * scroll;

            Camera.main.transform.position += Camera.main.transform.forward * displacement;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            float distance = 0;
            mPlane.Raycast(ray, out distance);

            if (distance < minScrollDistance)
            {
                Camera.main.transform.position = ray.GetPoint(distance - minScrollDistance);
            }
            else if (distance > maxScrollDistance)
            {
                Camera.main.transform.position = ray.GetPoint(distance - maxScrollDistance);
            }
        }

    }
}


