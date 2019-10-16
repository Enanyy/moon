using System.Threading;
/// <summary>
/// 无锁队列
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentQueue<T>
{
    internal class Node
    {
        internal T Item;
        internal Node Next;

        public Node(T item)
        {
            this.Item = item;
            this.Next = null;
        }
    }

    private Node mHead;
    private Node mTail;

    public ConcurrentQueue()
    {
        mHead = new Node(default(T));
        mTail = mHead;
    }

    public bool isEmpty
    {
        get { return (mHead.Next == null); }
    }

    public void Enqueue(T item)
    {
        Node newNode = new Node(item);
        while (true)
        {
            Node curTail = mTail;
            Node residue = curTail.Next;

            //判断_tail是否被其他process改变
            if (curTail == mTail)
            {
                //A 有其他process执行C成功，_tail应该指向新的节点
                if (residue == null)
                {
                    //C 如果其他process改变了tail.next节点，需要重新取新的tail节点
                    if (Interlocked.CompareExchange(ref curTail.Next, newNode, residue) == residue)
                    {
                        //D 尝试修改tail
                        Interlocked.CompareExchange(ref mTail, newNode, curTail);
                        return;
                    }
                }
                else
                {
                    //B 帮助其他线程完成D操作
                    Interlocked.CompareExchange(ref mTail, residue, curTail);
                }
            }
        }
    }

    public bool TryDequeue(out T result)
    {
        Node curHead;
        Node curTail;
        Node next;
        do
        {
            curHead = mHead;
            curTail = mTail;
            next = curHead.Next;
            if (curHead == mHead)
            {
                if (next == null)  //Queue为空
                {
                    result = default(T);
                    return false;
                }
                if (curHead == curTail) //Queue处于Enqueue第一个node的过程中
                {
                    //尝试帮助其他Process完成操作
                    Interlocked.CompareExchange(ref mTail, next, curTail);
                }
                else
                {
                    //取next.Item必须放到CAS之前
                    result = next.Item;
                    //如果_head没有发生改变，则将_head指向next并退出
                    if (Interlocked.CompareExchange(ref mHead, next, curHead) == curHead)
                        break;
                }
            }
        }
        while (true);
        return true;
    }
}
