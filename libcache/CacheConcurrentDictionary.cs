namespace Kontur.Cache
{
    using Kontur.Cache.ConcurrentPriorityQueue;
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    public class CacheConcurrentDictionary<T> : IMemoryCache<T>
    {
        private readonly ConcurrentDictionary<string, CacheItem<T>> map;
        private readonly ConcurrentPriorityQueue<CacheItem<T>> priorityQueue;
        private readonly TimeSpan cleanInterval;
        private readonly Timer cleanTimer;

        public CacheConcurrentDictionary(TimeSpan cleanInterval)
        {
            priorityQueue = new ConcurrentPriorityQueue<CacheItem<T>>();
            map = new ConcurrentDictionary<string, CacheItem<T>>();
            cleanTimer = new Timer(this.OnTimer);
            this.cleanInterval = cleanInterval;
            cleanTimer.Change(cleanInterval, TimeSpan.FromMilliseconds(-1));
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
            CacheItem<T> item;

            if (map.TryGetValue(key, out item))
            {
                if (!item.IsExpired())
                {
                    value = item.Value;
                    return true;
                }

                map.TryRemove(key, out item);
            }

            value = default(T);
            return false;
        }

        public void Insert(string key, T value, TimeSpan timeToLive)
        {
            var item = new CacheItem<T>(key, value, timeToLive);
            map[key] = item;
            priorityQueue.Enqueue(item, item.ExpiredTime);
        }

        public bool Remove(string key)
        {
            CacheItem<T> item;
            return map.TryRemove(key, out item);
        }

        public bool Contains(string key)
        {
            return map.ContainsKey(key);
        }

        public void Clean()
        {
            Console.WriteLine("Clean on");
            Console.WriteLine(priorityQueue.Length);
            Console.WriteLine(map.Count);

            while (this.priorityQueue.Length > 0 && this.priorityQueue.Peek().ExpiredTime < DateTime.Now)
            {
                CacheItem<T> item;
                map.TryRemove(priorityQueue.Dequeue().Key, out item);
            }

            Console.WriteLine("Clean off");
        }

        private void OnTimer(object state)
        {
            var dateTime = DateTime.Now;

            if (MemoryPressure.MemoryLoad > 90)
            {
                try
                {
                    this.PressureClean();
                }

                finally
                {
                    var cleanTime = DateTime.Now.Subtract(dateTime);
                    cleanTimer.Change(cleanInterval, TimeSpan.FromMilliseconds(-1));
                }
            }

            else
            {
                try
                {
                    this.Clean();
                }

                finally
                {
                    var cleanTime = DateTime.Now.Subtract(dateTime);
                    cleanTimer.Change(cleanInterval, TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        private void PressureClean()
        {
            this.Clean();
            int part = map.Count / 10;

            while (MemoryPressure.MemoryLoad >= 90)
            {
                for (int i = 0; i < part; i++)
                {
                    if (priorityQueue.Length == 0)
                    {
                        GC.Collect();
                        return;
                    }

                    this.Remove(this.priorityQueue.Dequeue().Key);
                }

                GC.Collect();
            }
        }
    }
}