using System;
using System.Collections.Generic;
using UnityEngine;
using PBMessage;



public class Main : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = 5000;
    public string username = "lwg";
    public string pwd = "1234";

    public ShapeType showType = ShapeType.None;

    public static Main Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        CameraManager.Instance.Init();
       
        AssetManager.Instance.LoadAsset<TextAsset>("data.bytes", (asset) => {

            if(asset!=null)
            {
                DataTableManager.Instance.Init(asset.assetObject.bytes);

            }
            else
            {
                Debug.LogError("Can't load data.bytes");
            }

        });
    }

    void Start()
    {
        Debug.Log("IsLittleEndian:"+BitConverter.IsLittleEndian);
        MessageManager.Instance.Init();
        NetworkManager.Instance.onReceive += MessageManager.Instance.OnReceive;
    }

    void LateUpdate()
    {
        NetworkManager.Instance.Update();
    }

    void Update()
    {
        BattleManager.Instance.Update(Time.deltaTime);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 100, 40), "Connect"))
        {
            NetworkManager.Instance.Connect((int)ConnectID.Logic, ip, port,
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
            MSG_LoginRequest request = MSG_LoginRequest.Get();

            request.message.name = username;
            request.message.password = pwd;


            request.Send(ConnectID.Logic);
        }
        if (user !=null&&GUI.Button(new Rect(20, 140, 100, 40), "Battle"))
        {

            MSG_BattleBeginRequest request = MSG_BattleBeginRequest.Get();
            request.Send(ConnectID.Logic);
        }
    }

    public UserData user = null;

    
    void OnApplicationQuit()
    {
        NetworkManager.Instance.Close();
    }

    
}


