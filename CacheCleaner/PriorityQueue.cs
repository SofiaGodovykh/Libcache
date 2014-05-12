namespace Kontur.Cache
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    public class PriorityQueue<TKey> : IEnumerable, IQueue<TKey> where TKey : IComparable
    {
        private readonly List<QueueNode<TKey>> prioQueue;

        private readonly ReaderWriterLockSlim queueLock;

        public PriorityQueue()
        {
            this.prioQueue = new List<QueueNode<TKey>>();
            this.prioQueue.Add(new QueueNode<TKey>(default(TKey), default(CacheItem)));
            queueLock = new ReaderWriterLockSlim();
        }

        public void Enqueue(QueueNode<TKey> q)
        {
            queueLock.EnterWriteLock();
            try
            {
                this.prioQueue.Add(q);
                int i = prioQueue.Count - 1;
                QueueNode<TKey> temp;
                while (i > 1 && this.prioQueue[i].Key.CompareTo(this.prioQueue[(i / 2)].Key) < 0)
                {
                    temp = this.prioQueue[i];
                    this.prioQueue[i] = this.prioQueue[i / 2];
                    this.prioQueue[i / 2] = temp;
                    i = i / 2;
                }
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
                if (this.prioQueue.Count == 1)
                {
                    throw new Exception("the queue is empty");
                }

                var x = this.prioQueue[1];
                this.prioQueue[1] = this.prioQueue[this.prioQueue.Count - 1];
                this.prioQueue.RemoveAt(this.prioQueue.Count - 1);
                int i = 1;
                int j;

                QueueNode<TKey> temp;
                while (2 * i <= (prioQueue.Count - 1) && i >= 1)
                {
                    if ((2 * i) < this.prioQueue.Count - 1
                        && this.prioQueue[(2 * i) + 1].Key.CompareTo(this.prioQueue[2 * i].Key) < 0)
                    {
                        j = (2 * i) + 1;
                    }
                    else
                    {
                        j = 2 * i;
                    }


                    if (this.prioQueue[i].Key.CompareTo(this.prioQueue[j].Key) > 0)
                    {
                        temp = this.prioQueue[i];
                        this.prioQueue[i] = this.prioQueue[j];
                        this.prioQueue[j] = temp;
                        i = j;
                    }
                    else
                    {
                        i = prioQueue.Count - 1;
                    }
                }

                return x;
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
                if (prioQueue.Count == 1)
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

        public IEnumerator GetEnumerator()
        {
            return prioQueue.GetEnumerator();
        }
    }
}
