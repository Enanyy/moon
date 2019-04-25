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

    

    public static T DeserializeProtoBuf<T>(byte[] data,int index, int count,T instance) where T : class, ProtoBuf.IExtensible
    {
        if (data == null)
        {
            return null;
        }
        using (MemoryStream ms = new MemoryStream(data,index,count))
        {
            T t = (T)ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, instance, typeof(T));
            return t;
        }
    }
    
}

