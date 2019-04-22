using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class ProtoTransfer
{
    public static byte[] SerializeProtoBuf<T>(T data) where T : class, ProtoBuf.IExtensible
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize<T>(ms, data);
            byte[] bytes = ms.ToArray();

            ms.Close();

            return bytes;
        }
    }

    

    public static T DeserializeProtoBuf<T>(byte[] data) where T : class, ProtoBuf.IExtensible
    {
        if (data == null)
        {
            return null;
        }
        using (MemoryStream ms = new MemoryStream(data))
        {
            T t = DeserializeProtoBuf<T>(ms);
            return t;
        }
    }
    public static T DeserializeProtoBuf<T>(byte[] data,int index, int count) where T : class, ProtoBuf.IExtensible
    {
        if (data == null)
        {
            return null;
        }
        using (MemoryStream ms = new MemoryStream(data,index,count))
        {
            T t = DeserializeProtoBuf<T>(ms);
            return t;
        }
    }
    public static T DeserializeProtoBuf<T>(MemoryStream ms) where T : class, ProtoBuf.IExtensible
    {
        T t = ProtoBuf.Serializer.Deserialize<T>(ms);
        ms.Dispose();
        return t;
    }
    public static object DeserializeProtoBuf(byte[] data, Type type)
    {
        if (data == null)
        {
            return null;
        }

        using (MemoryStream ms = new MemoryStream(data))
        {
            return DeserializeProtoBuf(ms,type);
        }
    }
    public static object DeserializeProtoBuf(MemoryStream ms, Type type)
    {
        if (ms == null)
        {
            return null;
        }

        object o = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, null, type);
        ms.Dispose();
        return o;
    }
}

