using System;
using System.Collections.Generic;

/// <summary>
/// 网络包的封装
/// </summary>
public class NetPacket
{
    private byte[] mData;
    public int ID { get; private set; }

    /// <summary>
    /// 整个包的字节长度，可能并没有填充满数据，根据Position来读取有效数据
    /// </summary>
    public byte[] data { get { return mData; } }

    /// <summary>
    /// 数据的长度
    /// </summary>
    public int Position { get; private set; }
    //字节数组的长度，可能并没有填充满数据
    public int Length { get { return data == null ? 0 : data.Length; } }

    public bool isPool { get ; private set; }

    public const int PACKET_ID_OFFSET = 0;//包ID偏移
    public const int PACKET_ID_LENGTH = 4;//包ID字节长度
    public const int PACKET_BUFFER_OFFSET = 4;//包数据偏移

    private static Queue<NetPacket> packetQueue = new Queue<NetPacket>();

    public static NetPacket Create()
    {
        NetPacket packet = null;
        if(packetQueue.Count >0)
        {
            packet = packetQueue.Dequeue();
        }
        else
        {
            packet = new NetPacket();
        }
        packet.Clear();
        packet.isPool = false;
        return packet;
    }
    public static NetPacket Create(byte[] _data)
    {
        NetPacket packet = Create();
       
        packet.SetData(_data);
        return packet;
    }
    public static NetPacket Create(int id, byte[] _data)
    {
        NetPacket packet = Create();

        packet.SetData(id, _data);
        return packet;
    }
    public static NetPacket Create(int id, byte[] _data, int startIndex, int length)
    {
        NetPacket packet = Create();

        packet.SetData(id, _data, startIndex, length);
        return packet;
    }

    public static void Recycle(NetPacket packet)
    {
        if(packet!=null)
        {
            packet.isPool = true;
            packetQueue.Enqueue(packet);
        }
    }

    static void ReAlloc(ref byte[] ba, int pos, int size)
    {
        if (ba == null)
        {
            ba = new byte[size];
        }
        else
        {
            if (ba.Length < (pos + size))
            {
                Array.Resize<byte>(ref ba, (int)(ba.Length + size + 1024));
            }
        }
    }

    private NetPacket() { Position = 0; }

    public void Clear() { Position = 0; }

    public void SetData(byte[] _data)
    {
        if (_data != null)
        {
            ReAlloc(ref mData, 0, _data.Length);
            Array.Copy(_data, 0, mData, 0, _data.Length);
            Position = _data.Length;
        }

        if (mData != null && mData.Length >= PACKET_ID_LENGTH)
        {
            ID = BitConverter.ToInt32(mData, PACKET_ID_OFFSET);
        }
    }

    public void SetData(int id, byte[] _data)
    {
        ID = id;
        if(_data==null)
        {
            ReAlloc(ref mData, 0, PACKET_ID_LENGTH);
            Array.Copy(BitConverter.GetBytes(id), 0, mData, PACKET_ID_OFFSET, PACKET_ID_LENGTH);
            Position = PACKET_ID_LENGTH;
        }
        else
        {
            ReAlloc(ref mData, 0, PACKET_ID_LENGTH + _data.Length);
            Array.Copy(BitConverter.GetBytes(id), 0, mData, PACKET_ID_OFFSET, PACKET_ID_LENGTH);
            Array.Copy(_data, 0, mData, PACKET_BUFFER_OFFSET, _data.Length);
            Position = PACKET_ID_LENGTH + _data.Length;
        }
    }

    public void SetData(int id, byte[] _data, int startIndex, int length)
    {
        ID = id;
        if (_data == null)
        {
            ReAlloc(ref mData, 0, PACKET_ID_LENGTH);
            Array.Copy(BitConverter.GetBytes(id), 0, mData, PACKET_ID_OFFSET, PACKET_ID_LENGTH);
            Position = PACKET_ID_LENGTH;
        }
        else
        {
            ReAlloc(ref mData, 0, PACKET_ID_LENGTH + length);
            Array.Copy(BitConverter.GetBytes(id), 0, mData, PACKET_ID_OFFSET, PACKET_ID_LENGTH);
            Array.Copy(_data, startIndex, mData, PACKET_BUFFER_OFFSET, length);
            Position = PACKET_ID_LENGTH + length;
        }
    }
}

