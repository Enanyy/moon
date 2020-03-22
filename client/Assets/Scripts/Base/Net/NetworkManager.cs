using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void PacketHandler(Packet packet);
public class Packet
{
    private static ConcurrentQueue<Packet> concurrentQueue = new ConcurrentQueue<Packet>();
    public static Packet CreatePacket(int size = 1024)
    {
        Packet packet;
        if (concurrentQueue.TryDequeue(out packet) == false)
        {
            packet = new Packet(size);
        }
        packet.Clear();
        return packet;
    }
    public static void ReturnPacket(Packet packet)
    {
        if(packet==null)
        {
            return;
        }
        concurrentQueue.Enqueue(packet);
    }

    public static void ClearPacket()
    {
        while(concurrentQueue.TryDequeue(out Packet packet))
        {
            packet.Clear();
        }
    }


    public Connection connection;
    private byte[] mData;
    public byte[] data { get { return mData; } }

    public int position { get; protected set; }

    public int length { get { return mData != null ? mData.Length : 0; } }

    public Packet(int size)
    {
        position = 0;
        Resize(size);
    }

    public void Write(byte[] sourceArray, int sourceIndex,int length)
    {
        if(sourceArray == null)
        {
            return;
        }

        Resize(position + length);

        Array.Copy(sourceArray, sourceIndex, mData, position, length);

        position += length;
    }

    public void Write(System.IO.MemoryStream stream, int length)
    {
        if(stream == null)
        {
            return;
        }
        Resize(position + length);

        stream.Read(mData, position, length);

        position += length;
    }

    public void Resize(int size)
    {
        if(mData == null)
        {
            mData = new byte[Mathf.NextPowerOfTwo(size)];
        }
        else if(mData.Length < size)
        {
            Array.Resize(ref mData, Mathf.NextPowerOfTwo(size));
        }
    }

    public void Clear()
    {
        position = 0;
        connection = null;
    }
}

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

    private ConcurrentQueue<Packet> mPacketList = new ConcurrentQueue<Packet>();
  
    private Queue<Connection> mConnectResults = new Queue<Connection>();
    private Dictionary<int, OnConnectionHandler> mConnectHandlerDic = new Dictionary<int, OnConnectionHandler>();
    private Dictionary<int, OnConnectionHandler> mDisconnectHandlerDic = new Dictionary<int, OnConnectionHandler>();

    public event PacketHandler onReceive;
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
        while (mPacketList.isEmpty == false)
        {
            Packet data;
            if(mPacketList.TryDequeue(out data))
            {
                //这里分发网络包
                if (onReceive != null)
                {
                    onReceive(data);
                }
                Packet.ReturnPacket(data);
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
    public void Send(int connectid, Packet packet)
    {
        if (packet == null)
        {
            return;
        }

        var client = GetConnection(connectid);

        if (client == null)
        {
            Packet.ReturnPacket(packet);
            return;
        }
       
        client.Send(packet);

        Packet.ReturnPacket(packet);

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

        Packet.ClearPacket();
    }

    private void OnMessage(Packet packet)
    {
        if (packet == null)
        {
            return;
        }
        
        mPacketList.Enqueue(packet);
        

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
}

