using System;
using System.Collections.Generic;

public interface IMessage
{
    MessageID id { get; set; }
    void OnReceive(NetPacket packet);
}

public abstract class Message<T> : IMessage where T : class, ProtoBuf.IExtensible, new()
{
    public MessageID id { get; set; }
    public T data { get; set; }

    public Message(MessageID id)
    {
        this.id = id;
        this.data = new T();
    }

    protected abstract void OnMessage();


    public void Send(ConnectID connectid)
    {
        NetworkManager.Instance().Send(connectid, id, data);
    }

    public void OnReceive(NetPacket packet)
    {
        data = ProtoTransfer.DeserializeProtoBuf<T>(packet.data,
            NetPacket.PACKET_BUFFER_OFFSET, packet.Position - NetPacket.PACKET_BUFFER_OFFSET);
        OnMessage();
    }
}

public class MessageManager
{
    private static MessageManager mInstance;

    public static MessageManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new MessageManager();
            }

            return mInstance;
        }
    }

    private Dictionary<int,IMessage> mMessageDic = new Dictionary<int,IMessage>();

    public void Init()
    {
        Register(new MSG_LoginRequest());
    }

    public void Register( IMessage message)
    {
        int id =(int) message.id;
        if (mMessageDic.ContainsKey(id) == false)
        {
            mMessageDic.Add(id, message);
        }
        else
        {
           UnityEngine.Debug.LogError("重复注册消息:"+id);
        }
    }

    public T Get<T>(MessageID id) where T :class, IMessage
    {
        IMessage message = null;
        mMessageDic.TryGetValue((int) id, out message);
        return message as T;
    }

    public void OnReceive(NetPacket packet)
    {
        int id = packet.ID;
        if (mMessageDic.ContainsKey(id))
        {
            mMessageDic[id].OnReceive(packet);
        }
        else
        {
            UnityEngine.Debug.LogError("消息ID:" + (MessageID)id+"未注册");
        }
    }


}

