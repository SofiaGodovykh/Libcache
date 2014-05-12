namespace Kontur.Cache
{
    using System;

    public sealed class CacheItem<T>
    {
        private readonly T value;
        private readonly string key;
        private readonly DateTime expiredTime;

        public CacheItem(string key, T value, TimeSpan timeToLive)
        {
            this.key = key;
            this.value = value;
            this.expiredTime = DateTime.Now.Add(timeToLive);
        }

        public T Value
        {
            get { return value; }
        }

        public DateTime ExpiredTime
        {
            get { return expiredTime; }
        }

        public string Key
        {
            get { return key; }
        }

        public bool IsExpired()
        {
            return IsExpired(DateTime.Now);
        }

        public bool IsExpired(DateTime dateTime)
        {
            return dateTime > expiredTime;
        }
    }
}