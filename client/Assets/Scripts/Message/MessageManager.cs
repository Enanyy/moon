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
    void Receive(byte[] data, int index, int length);
}

public abstract class Message<T> : IMessage where T : class, ProtoBuf.IExtensible, new()
{
    public MessageID id { get; set; }
    public T message { get; set; }

    public Message(MessageID id)
    {
        this.id = id;
        this.message = new T();
    }



    public void Send(ConnectID connectid)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize<T>(ms, message);

            int length = 4 + (int)ms.Position;
            byte[] packet = new byte[length];

            Array.Copy(BitConverter.GetBytes((int)id), 0, packet, 0, 4);
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(packet, 4, length - 4);
           
            ms.Close();
            
            NetworkManager.Instance.Send((int)connectid, packet);

        }

    }

    protected abstract void OnRecv();

    public void Receive(byte[] data, int index, int length)
    {
        using (MemoryStream ms = new MemoryStream(data,index, length))
        {
            message = (T)ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, message, typeof(T));
            OnRecv();
        }
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
//REGISTER_MESSAGE_START		Register(new MSG_LoginRequest());
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
        IMessage message = null;
        mMessageDic.TryGetValue((int) id, out message);
        return message as T;
    }

    public void OnReceive(Connection connection, byte[] packet)
    {
        if(packet == null)
        {
            return;
        }

        if (packet.Length >= 4)
        { 
            int id = BitConverter.ToInt32(packet, 0);
            if (mMessageDic.ContainsKey(id))
            {
                mMessageDic[id].Receive(packet, 4, packet.Length - 4);
            }
            else
            {
                UnityEngine.Debug.LogError("消息ID:" + (MessageID)id + "未注册");
            }
        } 
    }
}

