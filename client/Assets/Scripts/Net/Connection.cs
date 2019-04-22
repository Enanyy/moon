using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

public delegate void OnConnectionHandler(Connection c);
public delegate void OnReceiveHandler(byte[] data);
public delegate void OnDebugHandler(string msg);

public class Connection
{
    // 客户端接收缓存大小
    private const ushort MAX_NET_BUFFER = 1024 * 16;

    // 临时用，解包
    private byte[] mCopyData;

    // 接收缓存，存放数据
    private byte[] mRecvData;

    // 长度数据，存放长度信息
    private byte[] mLengthData;

    // 当前数据长度
    int mDataNowLength;

    /// <summary>
    /// 套接字
    /// </summary>
    private Socket mSocket;

    /// <summary>
    /// 是否正在连接
    /// </summary>
    public bool IsConnecting { get; private set; }

    // 已连接
    public bool IsConnected { get; private set; }


    private MemoryStream mStream;

    public ConnectID ID { get; private set; }
    public string IP { get; private set; }
    public int Port { get; private set; }

    public event OnConnectionHandler onConnect;
    public event OnConnectionHandler onDisconnect;
    public event OnReceiveHandler onReceive;
    public event OnDebugHandler onDebug;

    /// <summary>
    /// 发送/接收数据数据时，填充数据长度的两个字节是否使用小端
    /// </summary>
    public static bool IsLittleEndian = false;

    public Connection(ConnectID id)
    {
        ID = id;
        IsConnecting = false;
        IsConnected = false;

        mRecvData = new byte[MAX_NET_BUFFER];
        mCopyData = new byte[MAX_NET_BUFFER];

        mDataNowLength = 0;
        mLengthData = new byte[sizeof(ushort)];

        mStream = new MemoryStream();
    }



    public bool Reconnect()
    {

        Close(true);

        return Connect(IP, Port);
    }

    public void Update() { }

    // 连接到指定地址
    public bool Connect(string ip, int nPort)
    {
        IP = ip;
        Port = nPort;

        Log(string.Format("Connection::Start Connect ip={0}, port={1}", ip, nPort));

        // 如果已经连接或者正在连接，直接返回
        if (IsConnecting || IsConnected)
        {
            return true;
        }

        IsConnecting = true;

        try
        {
            IPAddress[] address = Dns.GetHostAddresses(ip);

            for (int i = 0; i < address.Length; ++i)
            {
                Log("Connection::address=" + address[i]);
            }

            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                Log("Connection::Connect InterNetworkV6");

            }
            else
            {
                Log("Connection::Connect InterNetwork");
            }

            mSocket = new Socket(address[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            mSocket.BeginConnect(address[0], nPort, OnEndConnect, null);
        }
        catch (Exception e)
        {
            Log(string.Format("Connection::Connect catch a exception, msg={0}", e.Message));

            Close();


            return false;
        }

        Log("Connection::Connecting....Please wait for result.");

        return true;
    }

    // 发送数据
    public bool Send(byte[] data, ushort length)
    {
        if (!IsConnected)
        {
            return false;
        }

        ushort nLength = length;    // 在最前面写入一个2个字节的数据长度
        byte[] lenData = BitConverter.GetBytes(nLength);
        if (IsLittleEndian == false)
        {
            Array.Reverse(lenData);
        }
        mStream.Seek(0, SeekOrigin.Begin);
        mStream.SetLength(0);

        // 写入数据大小
        mStream.Write(lenData, 0, lenData.Length);

        // 写入数据
        mStream.Write(data, 0, length);

        try
        {
            mSocket.BeginSend(mStream.ToArray(), 0, (int)mStream.Length, SocketFlags.None, OnEndSend, null);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == (int)SocketError.WouldBlock)
            {
                Log("Connection::SendData SocketException SocketError.WouldBlock ...");
                return true;
            }

            Log(string.Format("Connection::SendData SocketException catch a exception, msg={0}", e.Message));

            Close();

        }
        catch (Exception e)
        {
            Log(string.Format("Connection::SendData catch a exception, msg={0}", e.Message));

            Close();


        }

        return true;
    }

    public void Receive()
    {
        try
        {
            if (mSocket != null)
            {
                mSocket.BeginReceive(mRecvData, mDataNowLength, mRecvData.Length - mDataNowLength, SocketFlags.None, OnEndReceive, null);
            }
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == (int)SocketError.WouldBlock)
            {
                Log("Connection::Receive SocketException SocketError.WouldBlock ...");
                return;
            }

            Log(string.Format("Connection::Receive SocketException catch a exception, msg={0}", e.Message));

            Close();


        }
        catch (Exception e)
        {
            Log("Connection::Receive catch exception, msg=" + e.Message);

            Close();


        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    /// <param name="initiatively">主动关闭连接不会调用断开回调</param>
    public void Close(bool initiatively = false)
    {
        if (mSocket != null)
        {
            try
            {
                mSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            try
            {
                mSocket.Close();
            }
            catch
            {
            }

            mSocket = null;
        }

        mDataNowLength = 0;
        IsConnected = false;
        IsConnecting = false;

        Log("Connection::Socket Close !");
        if (initiatively == false)
        {
            if (onDisconnect != null)
            {
                onDisconnect(this);
            }
        }
    }


    //////////////////////////////////////////////////////////////////////////
    // 已连接回调
    private void OnEndConnect(IAsyncResult iar)
    {
        IsConnecting = false;

        try
        {
            mSocket.EndConnect(iar);

            IsConnected = true;

            //m_Socket.NoDelay = true;

            //m_Socket.ReceiveBufferSize = MAX_NET_BUFFER;
            //m_Socket.SendBufferSize = MAX_NET_BUFFER;

            if (onConnect != null)
            {
                onConnect(this);
            }

            Receive();
        }
        catch (Exception e)     // 该try catch不能去掉，因为异步操作，有可能会返回异常
        {
            Log("Connection::OnConnected catch a exception " + e.Message);

            Close();

        }
    }

    // 发送数据回调
    private void OnEndSend(IAsyncResult result)
    {
        try
        {
            int count = mSocket.EndSend(result);

        }
        catch (SocketException e)
        {
            if (e.ErrorCode == (int)SocketError.WouldBlock)
            {
                Log("Connection::OnSendData SocketException SocketError.WouldBlock ...");
                return;
            }

            Log(string.Format("Connection::OnSendData SocketException catch a exception, msg={0}", e.Message));

            Close();


        }
        catch (Exception e)     // 该try catch不能去掉，因为异步操作，有可能会返回异常
        {
            Log(string.Format("Connection::OnSendData catch a exception msg={0}", e.Message));
            Close();


        }

    }


    private void OnEndReceive(IAsyncResult result)
    {
        try
        {
            int nReceive = mSocket.EndReceive(result);

            OnReceiveData(nReceive);

            Receive();
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == (int)SocketError.WouldBlock)
            {
                Log("Connection::OnEndReceive SocketException SocketError.WouldBlock ...");
                return;
            }

            Log(string.Format("Connection::OnEndReceive SocketException catch a exception, msg={0}", e.Message));

            Close();


        }
        catch (Exception e)     // 该try catch不能去掉，因为异步操作，有可能会返回异常
        {
            Log(string.Format("Connection::OnEndReceive catch a exception msg={0}", e.Message));
            Close();
        }
    }

    // 接收数据回调
    private void OnReceiveData(int nRecvLen)
    {
        // 总数据长度
        int nTotalLen = mDataNowLength + nRecvLen;
        if (nTotalLen <= sizeof(ushort))   // 数据太小
        {
            mDataNowLength += nRecvLen;
            return;
        }

        // 读出两个字节表示的数据长度
        if (IsLittleEndian ==false)
        {
            //大端
            mLengthData[0] = mRecvData[1];
            mLengthData[1] = mRecvData[0];
        }
        else
        {
            //小端
            mLengthData[0] = mRecvData[0];
            mLengthData[1] = mRecvData[1];
        }

        int nPackageLen = BitConverter.ToUInt16(mLengthData, 0);

        // 如果总长度 < 数据长度+ 数据头， 说明数据还未接收完
        if (nTotalLen < (int)(nPackageLen + sizeof(ushort)))
        {
            mDataNowLength += nRecvLen;
            return;
        }
        else if (nTotalLen == nPackageLen + sizeof(ushort))     // 刚好收到一个包
        {
            if (null != onReceive)
            {
                byte[] packetData = new byte[nPackageLen];
                Array.Copy(mRecvData, sizeof(ushort), packetData, 0, nPackageLen);

                //TRACE.Log(string.Format("m_InfoList.Push(packetData) 1 nPackageLen={0}", nPackageLen));
                onReceive(packetData);
            }

            mDataNowLength = 0;
            return;
        }
        else    // 收到了多个包，要把包都拆出来
        {
            Array.Copy(mRecvData, mCopyData, nTotalLen);     // 把已经接收到的数据备份一下
            int nReadPos = 0;
            while (true)
            {
                // 先拷贝一个数据包，提交到列表中
                if (null != onReceive)
                {
                    byte[] packetData = new byte[nPackageLen];
                    Array.Copy(mCopyData, nReadPos + sizeof(ushort), packetData, 0, nPackageLen);


                    onReceive(packetData);
                }

                nReadPos += nPackageLen + sizeof(ushort);
                mDataNowLength = nTotalLen - nReadPos;  // 剩余数据长度

                if (mDataNowLength <= sizeof(ushort))       // 如果剩余的长度不够两个字节
                {
                    Array.Copy(mCopyData, nReadPos, mRecvData, 0, mDataNowLength);       // 把剩余的数据拷贝回去，等待下剩余的数据
                    break;
                }
                else    // 如果超过两个字节
                {
                    mLengthData[0] = mRecvData[nReadPos];
                    mLengthData[1] = mRecvData[nReadPos + 1];
                    nPackageLen = BitConverter.ToUInt16(mLengthData, 0);       // 得到下一个包的数据长度

                    // 否则就把数据拷贝回去，等待剩余数据
                    if (mDataNowLength < nPackageLen + sizeof(ushort))
                    {
                        Array.Copy(mCopyData, nReadPos, mRecvData, 0, mDataNowLength);
                        break;
                    }
                }
            }
        }
    }

    private void Log(string msg)
    {
        if (onDebug != null)
        {
            onDebug(msg);
        }
    }
}
