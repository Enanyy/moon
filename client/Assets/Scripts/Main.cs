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

        Drag();
        Scroll();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 100, 40), "Connect"))
        {
            NetworkManager.Instance.Connect(ConnectID.Logic, ip, port,
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


