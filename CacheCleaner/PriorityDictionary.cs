namespace Kontur.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class PriorityDictionary<TKey> : IQueue<TKey> where TKey : IComparable
    {
        private readonly SortedDictionary<TKey, Queue<QueueNode<TKey>>> dictionary;
        private readonly ReaderWriterLockSlim queueLock;

        public PriorityDictionary()
        {
            dictionary = new SortedDictionary<TKey, Queue<QueueNode<TKey>>>();
            queueLock = new ReaderWriterLockSlim();
        }

        public void Enqueue(QueueNode<TKey> q)
        {
            Queue<QueueNode<TKey>> queue;
            queueLock.EnterWriteLock();
            try
            {
                if (!dictionary.TryGetValue(q.Key, out queue))
                {
                    queue = new Queue<QueueNode<TKey>>();
                    dictionary.Add(q.Key, queue);
                }

                queue.Enqueue(q);
            }
            finally
            {
                queueLock.ExitWriteLock();
            }
        }

        public QueueNode<TKey> Dequeue()
        {
            queueLock.EnterWriteLock();
            try
            {
                var pair = dictionary.First();
                var v = pair.Value.Dequeue();

                if (pair.Value.Count == 0)
                {
                    dictionary.Remove(pair.Key);
                }

                return v;
            }
            finally
            {
                queueLock.ExitWriteLock();
            }
        }

        public bool IsEmpty()
        {
            queueLock.EnterReadLock();
            try
            {
                if (dictionary.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            finally
            {
                queueLock.ExitReadLock();
            }
        }
    }
}
