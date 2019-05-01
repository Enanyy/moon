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
    public byte[] data { get; private set; }

 
    //数据的实际长度
    public int length { get; set; }

    public bool isPool { get ; private set; }

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
        packet.isPool = false;

        return packet;
    }
    
    public static void Recycle(NetPacket packet)
    {
        if(packet!=null)
        {
            packet.isPool = true;
            mPacketQueue.Enqueue(packet);
        }
    }


    private NetPacket(int size)
    {
        Resize(size);
        length = 0;
    }

    public void Clear() { length = 0; }

    public void Resize(int size)
    { 
        if (data == null)
        {
            data = new byte[size];
        }
        else
        {
            if (data.Length < size)
            {
                var buffer = data;
                Array.Resize(ref buffer, size);
            }
        }

    }
}

