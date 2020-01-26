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

    /// <summary>
    /// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
    /// </summary>
    public void Init()
    {
//REGISTER_MESSAGE_BEGIN		Register(new MSG_LoginRequest());
		Register(new MSG_LoginReturn());
		Register(new MSG_LoginGameNotify());
		Register(new MSG_LoginGameRequest());
		Register(new MSG_LoginGameReturn());
		Register(new MSG_BattleBeginRequest());
		Register(new MSG_BattleBeginReturn());
		Register(new MSG_BattleBeginNotify());
		Register(new MSG_BattleEntityIdleNotify());
		Register(new MSG_BattleEntityRunNotify());
		Register(new MSG_BattleEntityAttackNotify());
		Register(new MSG_BattleEntityAttackChangeNotify());
		Register(new MSG_BattleEntityDieNotify());
		Register(new MSG_BattleEntityBloodNotify());
		Register(new MSG_BattleEntityPropertyNotify());
		Register(new MSG_BattleEndNotify());
//REGISTER_MESSAGE_END
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

