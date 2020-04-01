using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;


public class MessageReciever 
{
    private MethodInfo method;
    private object target;
    private object[] param = new object[1];
    public MessageReciever(object target, MethodInfo method)
    {
        this.target = target;
        this.method = method;
    }
    public void Invoke(object obj)
    {
        param[0] = obj;

        method.Invoke(target, param);
    }
}
[AttributeUsage(AttributeTargets.Method)]
public class MessageRecieverAttribute : Attribute
{ 
    public MessageRecieverAttribute()
    {
    }
}

public class TestMessage
{
    public string name = "Hello";
}

public class TestMessage1
{
    public string name = "Hello 1";
}





public class MessageListener
{
    public object handler { get; private set; }
    public readonly Dictionary<Type, MessageReciever> recievers = new Dictionary<Type, MessageReciever>();

    public MessageListener(Type type)
    {
       handler = Activator.CreateInstance(type);
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 1)
            {
                MessageRecieverAttribute attr = (MessageRecieverAttribute)method.GetCustomAttribute(typeof(MessageRecieverAttribute), true);
                if (attr != null)
                {
                    var parameterType = parameters[0].ParameterType;
                    if (recievers.ContainsKey(parameterType) == false)
                    {
                        recievers.Add(parameterType, new MessageReciever(handler, method));
                    }
                }
            }
        }
    }

    public void Invoke(object obj)
    {
        if(recievers.TryGetValue(obj.GetType(),out MessageReciever reciever))
        {
            reciever.Invoke(obj);
        }
    }
}
[AttributeUsage(AttributeTargets.Class)]
public class TestMessageHandlerAttribute : Attribute
{

}
[TestMessageHandler]
public class TestMessageHandler
{
    [MessageReciever]
    public void OnTest(TestMessage msg)
    {
        Debug.Log(msg.name);
    }
    [MessageReciever]
    public void OnTest(TestMessage1 msg)
    {
        Debug.Log(msg.name);
    }
    public void OnTest(int i)
    {
        Debug.Log(i);

    }
    public void OnTest()
    {
        Debug.Log("null");
    }
}
public class TestAttributes : MonoBehaviour
{
    List<MessageListener> listeners = new List<MessageListener>();
    // Start is called before the first frame update
    TestMessage msg = new TestMessage();
    TestMessage1 msg1 = new TestMessage1();
    void Start()
    {
        List<Type> types = Class.GetTypes(GetType().Assembly, typeof(TestMessageHandlerAttribute), true);

        foreach (var type in types)
        {
            listeners.Add(new MessageListener(type));
        }


        foreach (var listener in listeners)
        {
            listener.Invoke(msg);
            listener.Invoke(msg1);
        }
        Action<int> action = null;
        var mt = action.GetMethodInfo();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < listeners.Count; ++i)
            {
                var listener = listeners[i];

                listener.Invoke(msg);
                listener.Invoke(msg1);

            }
        }
    }
}
