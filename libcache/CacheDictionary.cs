namespace Kontur.Cache
{
    using Kontur.Cache.ConcurrentPriorityQueue;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class CacheDictionary<T> : IMemoryCache<T>
    {
        private readonly TimeSpan cleanInterval;
        private readonly Timer cleanTimer;
        private readonly Dictionary<string, CacheItem<T>> map;
        private readonly ReaderWriterLockSlim cacheLock;
        private readonly ConcurrentPriorityQueue<CacheItem<T>> priorityQueue;

        public CacheDictionary(TimeSpan cleanInterval)
        {
            cleanTimer = new Timer(OnTimer);
            map = new Dictionary<string, CacheItem<T>>();
            cacheLock = new ReaderWriterLockSlim();
            priorityQueue = new ConcurrentPriorityQueue<CacheItem<T>>();

            cleanTimer = new Timer(this.OnTimer);
            this.cleanInterval = cleanInterval;
            cleanTimer.Change(cleanInterval, Timeout.InfiniteTimeSpan);
        }

        public int Count
        {
            get { return map.Count; }
        }

        public T GetValue(string key)
        {
            T value;
            TryGetValue(key, out value);
            return value;
        }

        public bool TryGetValue(string key, out T value)
        {
            CacheItem<T> item = null;

            cacheLock.EnterReadLock();
            try
            {
                if (map.TryGetValue(key, out item))
                {
                    if (!item.IsExpired())
                    {
                        value = item.Value;
                        return true;
                    }
                }

                value = default(T);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

            if (item != null)
            {
                Remove(key);
            }

            return false;
        }

        public void Insert(string key, T value, TimeSpan timeToLive)
        {
            cacheLock.EnterWriteLock();
            try
            {
                var item = new CacheItem<T>(key, value, timeToLive);
                map[key] = item;
                priorityQueue.Enqueue(item, item.ExpiredTime);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public bool Remove(string key)
        {
            cacheLock.EnterWriteLock();

            try
            {
                var res = map.Remove(key);
                return res;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public bool Contains(string key)
        {
            cacheLock.EnterReadLock();

            try
            {
                return map.ContainsKey(key);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public int Size()
        {
            return map.Count;
        }

        public void Clean()
        {
            Console.WriteLine("Cleaning on");

            try
            {
                cacheLock.EnterWriteLock();

                Console.WriteLine("Queue count: {0}", priorityQueue.Length);

                while (this.priorityQueue.Length > 0)
                {
                    var item = priorityQueue.Peek();

                    if (item.ExpiredTime > DateTime.Now)
                    {
                        break;
                    }

                    priorityQueue.Dequeue();
                    map.Remove(item.Key);
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }


            Console.WriteLine("Cleaning off");
        }

        private void OnTimer(object state)
        {
            // TODO: надо перерасчитать cleanInterval

            try
            {
                this.Clean();
            }
            finally
            {
                cleanTimer.Change(cleanInterval, Timeout.InfiniteTimeSpan);
            }
        }
    }
}