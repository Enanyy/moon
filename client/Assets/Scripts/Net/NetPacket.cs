using System;
using System.Collections.Generic;

/// <summary>
/// 网络包的封装
/// </summary>
public class NetPacket
{
    /// <summary>
    /// 整个包的字节长度，可能并没有填充满数据，根据length来读取有效数据
    /// </summary>
    private byte[] mBuffer;
    public byte[] Buffer { get { return mBuffer; } }

 
    //数据的实际长度
    public int Length { get; set; }


    private static Queue<NetPacket> mPacketQueue = new Queue<NetPacket>();
    public static NetPacket Create(int size)
    {
        NetPacket packet = null;
        if (mPacketQueue.Count > 0)
        {
            packet = mPacketQueue.Dequeue();
            packet.Resize(size);
        }
        else
        {
            packet = new NetPacket(size);
        }
        packet.Clear();

        return packet;
    }
    
    public static void Recycle(NetPacket packet)
    {
        if(packet!=null)
        {
            mPacketQueue.Enqueue(packet);
        }
    }


    private NetPacket(int size)
    {
        Resize(size);
        Length = 0;
    }

    public void Clear() { Length = 0; }

    public void Resize(int size)
    { 
        if (mBuffer == null)
        {
            mBuffer = new byte[size];
        }
        else
        {
            if (mBuffer.Length < size)
            {
                Array.Resize(ref mBuffer, size);
            }
        }

    }
}

