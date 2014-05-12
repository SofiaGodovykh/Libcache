namespace Kontur.Cache
{
    using System;

    public interface IQueue<TKey>
        where TKey : IComparable
    {
       void Enqueue(QueueNode<TKey> q);

        QueueNode<TKey> Dequeue();

        bool IsEmpty();
    }
}
