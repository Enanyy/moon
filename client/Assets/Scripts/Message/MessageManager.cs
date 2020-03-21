using System;
using System.Collections.Generic;
using System.IO;

public enum ConnectID
{
    Logic,
    Game,
}

public interface IMessage
{
    MessageID id { get; set; }
    void Receive(Connection connection, byte[] data, int index, int length);
}

public abstract class Message<T> : IMessage where T : class, ProtoBuf.IExtensible, new()
{
    public MessageID id { get; set; }
    public T message { get; set; }

    private static MemoryStream memoryStream = new MemoryStream();

    public Message(MessageID id)
    {
        this.id = id;
        this.message = new T();
    }



    public void Send(ConnectID connectid)
    {
        memoryStream.Seek(0, SeekOrigin.Begin);
        
        ProtoBuf.Serializer.Serialize<T>(memoryStream, message);

        int length = 4 + (int)memoryStream.Position;

        Packet packet = NetworkManager.Instance.GetOrCreatePacket(length);
        packet.Write(BitConverter.GetBytes((int)id), 0, 4);
        packet.Write(memoryStream, (int)memoryStream.Position);

        NetworkManager.Instance.Send((int)connectid, packet);
    }

    protected abstract void OnRecv(Connection connection);

    public void Receive(Connection connection, byte[] data, int index, int length)
    {
        memoryStream.Seek(0, SeekOrigin.Begin);
        message = (T)ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(memoryStream, message, typeof(T));
        OnRecv(connection);
    }
}
[AttributeUsage(AttributeTargets.Class)]
public class MessageHandlerAttribute : Attribute
{

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
        //根据MessageHandlerAttribute自动注册
        List<Type> types = Class.GetTypes(GetType().Assembly, typeof(MessageHandlerAttribute), true);
        for(int i = 0; i < types.Count;++i)
        {
            IMessage message =(IMessage)Activator.CreateInstance(types[i]);
            Register(message);
        }
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
        mMessageDic.TryGetValue((int)id, out IMessage message);
        return message as T;
    }

    public void OnReceive(Packet packet)
    {
        if(packet == null)
        {
            return;
        }

        if (packet.position >= 4)
        { 
            int id = BitConverter.ToInt32(packet.data, 0);
            if (mMessageDic.ContainsKey(id))
            {
                mMessageDic[id].Receive(packet.connection, packet.data, 4, packet.position - 4);
            }
            else
            {
                UnityEngine.Debug.LogError("消息ID:" + (MessageID)id + "未注册");
            }
        } 
    }
}

