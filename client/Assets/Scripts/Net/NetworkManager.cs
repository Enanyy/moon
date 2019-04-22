using System;
using System.Collections.Generic;
using UnityEngine;


public enum ConnectID
{
    Logic,
    Game,
}

public delegate void NetPacketHandler(NetPacket packet);

public class NetworkManager 
{
    #region Instance

    private static NetworkManager _instance;

    public static NetworkManager Instance()
    {
        if (_instance == null)
        {
            _instance = new NetworkManager();
        }

        return _instance;
    }

    #endregion

    private Dictionary<int, Connection> mConnectionDic = new Dictionary<int, Connection>();

    
    private Queue<NetPacket> mPacketList = new Queue<NetPacket>();
    private Queue<Connection> mConnectResults = new Queue<Connection>();
    private Dictionary<int, OnConnectionHandler> mConnectHandlerDic = new Dictionary<int, OnConnectionHandler>();
    private Dictionary<int, OnConnectionHandler> mDisconnectHandlerDic = new Dictionary<int, OnConnectionHandler>();
    private object mLock = new object();
    public event NetPacketHandler onReceive;
    private DateTime D1970 = new DateTime(1970, 1, 1, 0, 0, 0);


    public void Connect(ConnectID varConnectID, string ip, int port, OnConnectionHandler connectCallback, OnConnectionHandler disconnectCallback)
    {
        int id = (int)varConnectID;

        if (mConnectionDic.ContainsKey(id))
        {
            Connection client = mConnectionDic[id];
            mConnectionDic.Remove(id);
            client.Close(true);
        }
        if(mConnectHandlerDic.ContainsKey(id))
        {
            mConnectHandlerDic.Remove(id);
        }

        if(mDisconnectHandlerDic.ContainsKey(id))
        {
            mDisconnectHandlerDic.Remove(id);
        }

        Connection c = new Connection(varConnectID);

        c.onConnect += OnConnect;
        c.onDisconnect += OnDisconnect;
        c.onDebug += OnDebug;

        c.onReceive += OnMessage;

        mConnectionDic.Add(id, c);
        mConnectHandlerDic.Add(id, connectCallback);
        mDisconnectHandlerDic.Add(id, disconnectCallback);

        c.Connect(ip, port);
    }

    public Connection GetConnection(ConnectID clientID)
    {
        int id = (int)clientID;
        if (mConnectionDic.ContainsKey(id))
        {
            return mConnectionDic[id];
        }
        return null;
    }

    /// <summary>
    /// 主线程调用
    /// </summary>
    public void Update()
    {
        if (mPacketList.Count > 0)
        {
            lock (mLock)
            {
                while (mPacketList.Count > 0)
                {
                    NetPacket packet = mPacketList.Dequeue();
                    //这里分发网络包
                    if (onReceive != null)
                    {
                        onReceive(packet);
                    }
                    NetPacket.Recycle(packet);
                }
            }
        }
        while (mConnectResults.Count > 0)
        {
            var c = mConnectResults.Dequeue();
            int id = (int)c.ID;
            if (c.IsConnected)
            {
                if (mConnectHandlerDic.ContainsKey(id) && mConnectHandlerDic[id] != null)
                {
                    mConnectHandlerDic[id](c);
                }
            }
            else
            {
                if (mDisconnectHandlerDic.ContainsKey(id) && mDisconnectHandlerDic[id] != null)
                {
                    mDisconnectHandlerDic[id](c);
                }
            }
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientID"></param>
    /// <param name="id"></param>
    /// <param name="packet"></param>
    public void Send(ConnectID connectid, NetPacket packet)
    {
        if (packet == null)
        {
            return;
        }

        var client = GetConnection(connectid);

        if (client == null)
        {
            return;
        }
       
        client.Send(packet.data, (ushort)packet.Position);
        NetPacket.Recycle(packet);
        Debug.Log("NetworkManager.Send,id=" + packet.ID);
    }


    public void Send<T>(ConnectID connectid,MessageID msgid, T data) where T : class, ProtoBuf.IExtensible
    {
        byte[] buffer = ProtoTransfer.SerializeProtoBuf<T>(data);

        NetPacket packet = NetPacket.Create((int)msgid, buffer);

        Send(connectid, packet);
    }


    /// <summary>
    /// 断开连接
    /// </summary>
    public void Close(ConnectID connectid)
    {
        if (mConnectionDic.ContainsKey((int)connectid))
        {
            var c = mConnectionDic[(int)connectid];
            mConnectionDic.Remove((int)connectid);
            c.Close(true);
        }
    }

    /// <summary>
    /// 断开所有连接
    /// </summary>
    public void Close()
    {
        foreach (var v in mConnectionDic.Values)
        {
            v.Close(true);
        }
        mConnectionDic.Clear();
    }

    private void OnMessage(byte[] data)
    {
        if (data == null)
        {
            return;
        }

        NetPacket packet = NetPacket.Create(data);
        lock (mLock)
        {
            mPacketList.Enqueue(packet);
        }

    }
    private void OnConnect(Connection c)
    {
        mConnectResults.Enqueue(c);
    }

    private void OnDisconnect(Connection c)
    {
        mConnectResults.Enqueue(c);
    }

    private void OnDebug(string message)
    {
        //if (Debuger.EnableLog)
        {
            Debug.Log(message);
        }
    }

    private ulong m_nLoaclTime;
    private ulong m_nServerMillTime;

    public void UpdateServerTime(ulong nClientTime, ulong nServerTime)
    {
        m_nLoaclTime = (ulong)(DateTime.UtcNow - D1970).TotalMilliseconds;

        ulong rtt = m_nLoaclTime - nClientTime;

        m_nServerMillTime = nServerTime + rtt / 2;
    }

    /// <summary>
    /// 返回当前服务器毫秒数
    /// </summary>
    public ulong mCurServerMillTime
    {
        get
        {
            ulong nCurTime = (ulong)(DateTime.UtcNow - D1970).TotalMilliseconds;
            return m_nServerMillTime + (nCurTime - m_nLoaclTime);
        }

    }

    /// <summary>
    /// 当前服务器时间，秒
    /// </summary>
    public uint mCurServerTime
    {
        get
        {
            return (uint)(mCurServerMillTime / 1000);
        }
    }
}

