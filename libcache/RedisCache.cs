namespace Kontur.Cache
{
    using ServiceStack.Redis;
    using System;
    using System.IO;

    public class RedisCache<T> : ICache<T>
    {
        private readonly IBinarySerializer<T> serializer;

        private readonly RedisNativeClient client;

        public RedisCache(IBinarySerializer<T> serializer)
            : this(RedisNativeClient.DefaultHost, RedisNativeClient.DefaultPort, 0, serializer)
        {
        }

        public RedisCache(string host, int port, long db, IBinarySerializer<T> serializer)
        {
            this.serializer = serializer;
            client = new RedisNativeClient(host, port, null, db);
        }

        public bool TryGetValue(string key, out T value)
        {
            var rawValue = client.Get(key);

            if (rawValue != null)
            {
                var stream = new MemoryStream(rawValue);
                value = serializer.Deserialize(stream);
                return true;
            }

            value = default(T);
            return false;
        }

        public void Insert(string key, T value, TimeSpan timeToLive)
        {
            var stream = new MemoryStream();
            serializer.Serialize(stream, value);
            client.SetEx(key, (int)timeToLive.TotalSeconds, stream.ToArray());
        }

        public T GetValue(string key)
        {
            T value;
            this.TryGetValue(key, out value);
            return value;
        }

        public bool Remove(string key)
        {
            return client.Del(key) > 0;
        }

        public bool Contains(string key)
        {
            return client.Exists(key) == 1;
        }
    }
}