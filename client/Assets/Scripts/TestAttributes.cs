using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public interface IMessageReciever
{
    void AddReciever(object target, MethodInfo method);
    void Invoke(object obj);
}
public class MessageReciever<T> :IMessageReciever where T:class 
{
    private Action<T> reciever;
    public void AddReciever(object target, MethodInfo method)
    {
        reciever = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), target, method.Name);
    }
    public void Invoke(object obj)
    {
        if(reciever!=null)
        {
            reciever(obj as T);
        }
    }
}
[AttributeUsage(AttributeTargets.Method)]
public class MessageRecieverAttribute : Attribute
{
    public int id { get; private set; }
    public IMessageReciever reciever { get; private set; }
    public MessageRecieverAttribute(int id,Type type)
    {
        this.id = id;
        this.reciever = (IMessageReciever)Activator.CreateInstance(type);
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

[AttributeUsage(AttributeTargets.Class)]
public class MessageHandlerAttribute : Attribute
{

}



public class MessageListener
{
    public object handler { get; private set; }
    public readonly Dictionary<int, IMessageReciever> recievers = new Dictionary<int, IMessageReciever>();

    public MessageListener(Type type)
    {
       handler = Activator.CreateInstance(type);
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes(typeof(MessageRecieverAttribute), true);
            foreach (MessageRecieverAttribute attr in attributes)
            {
                if (recievers.ContainsKey(attr.id) == false)
                {
                    attr.reciever.AddReciever(handler, method);
                    recievers.Add(attr.id, attr.reciever);
                }
            }
        }
    }

    public void Invoke(int id,object obj)
    {
        IMessageReciever reciever;
        if(recievers.TryGetValue(id,out reciever))
        {
            reciever.Invoke(obj);
        }
    }
}

[MessageHandler]
public class TestMessageHandler
{
    [MessageReciever(1, typeof(MessageReciever<TestMessage>))]
    public void OnTest(TestMessage msg)
    {
        Debug.Log(msg.name);
    }
    [MessageReciever(2, typeof(MessageReciever<TestMessage1>))]
    public void OnTest(TestMessage1 msg)
    {
        Debug.Log(msg.name);
    }
}
public class TestAttributes : MonoBehaviour
{
    List<MessageListener> listeners = new List<MessageListener>();
    // Start is called before the first frame update
    void Start()
    {
        List<Type> types = Class.GetTypes(typeof(MessageHandlerAttribute), true);
        
        foreach(var type in types)
        {
            listeners.Add(new MessageListener(type));
        }

        TestMessage msg = new TestMessage();
        TestMessage1 msg1 = new TestMessage1();
        foreach(var listener in listeners)
        {
            listener.Invoke(1, msg);
            listener.Invoke(2, msg1);
        }
       
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
