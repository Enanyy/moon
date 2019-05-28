using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void MessageHandler(Connection c, byte[] packet);

public class NetworkManager 
{
    #region Instance

    private static NetworkManager _instance;

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NetworkManager();
            }

            return _instance;
        }
    }

    #endregion

    private Dictionary<int, Connection> mConnectionDic = new Dictionary<int, Connection>();

    
    private Queue<KeyValuePair<Connection, byte[]> > mPacketList = new Queue<KeyValuePair<Connection, byte[]>>();
    private Queue<Connection> mConnectResults = new Queue<Connection>();
    private Dictionary<int, OnConnectionHandler> mConnectHandlerDic = new Dictionary<int, OnConnectionHandler>();
    private Dictionary<int, OnConnectionHandler> mDisconnectHandlerDic = new Dictionary<int, OnConnectionHandler>();
    private object mLock = new object();
    public event MessageHandler onReceive;
    private DateTime D1970 = new DateTime(1970, 1, 1, 0, 0, 0);


    public void Connect(int id, string ip, int port, OnConnectionHandler connectCallback, OnConnectionHandler disconnectCallback)
    {
        
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

        Connection c = new Connection(id);

        c.onConnect += OnConnect;
        c.onDisconnect += OnDisconnect;
        c.onDebug += OnDebug;

        c.onReceive += OnMessage;

        mConnectionDic.Add(id, c);
        mConnectHandlerDic.Add(id, connectCallback);
        mDisconnectHandlerDic.Add(id, disconnectCallback);

        c.Connect(ip, port);
    }

    public Connection GetConnection(int id)
    {
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
                    var data = mPacketList.Dequeue();
                    //这里分发网络包
                    if (onReceive != null)
                    {
                        onReceive(data.Key, data.Value);
                    }
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
    public void Send(int connectid, byte[] packet)
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
       
        client.Send(packet, (ushort)packet.Length);

    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void Close(int connectid)
    {
        if (mConnectionDic.ContainsKey(connectid))
        {
            var c = mConnectionDic[connectid];
            mConnectionDic.Remove(connectid);
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

    private void OnMessage(Connection connection, byte[] packet)
    {
        if (packet == null)
        {
            return;
        }

        lock (mLock)
        {
            mPacketList.Enqueue(new KeyValuePair<Connection, byte[]>(connection,packet));
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

