namespace Kontur.Cache
{
    using System;

    public class QueueNode<TKey>
    {
        public readonly TKey Key;

        private readonly CacheItem value;

        public QueueNode(TKey p, CacheItem v)
        {
            this.Key = p;
            this.value = v;
        }

        public DateTime GetItemTime()
        {
            return value.ExpiredTime;
        }

        public string GetItemValue()
        {
            return value.Value;
        }
    }
}