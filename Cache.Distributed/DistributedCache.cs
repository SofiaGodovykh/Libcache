namespace Kontur.Cache.Distributed
{
    using ServiceStack.Redis;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DistributedCache<T> : ICache<T>
    {
        private readonly TopologyRing topologyRing;
        private readonly Dictionary<RedisEndPoint, RedisNativeClient> clients;
        private readonly IBinarySerializer<T> serializer;

        public DistributedCache(TopologyRing topologyRing, IBinarySerializer<T> serializer)
        {
            this.topologyRing = topologyRing;
            this.serializer = serializer;

            clients = new Dictionary<RedisEndPoint, RedisNativeClient>();

            foreach (var v in topologyRing.GetAllEndPoints())
            {
                clients.Add(v, new RedisNativeClient(v.Host, v.Port, null, v.Db));
            }
        }

        public bool TryGetValue(string key, out T value)
        {
            var endPoints = topologyRing.Resolve(key.GetHashCode());

            if (endPoints == null)
            {
                throw new Exception("Topology resolve exception");
            }

            else
            {
                Exception lastException = null;

                foreach (var client in endPoints)
                {
                    RedisNativeClient nativeClient;
                    clients.TryGetValue(client, out nativeClient);

                    if(nativeClient != null)
                    {
                        byte[] rawValue;

                        try
                        {
                            rawValue = nativeClient.Get(key);
                        }
                        catch (Exception exception)
                        {
                            lastException = exception;
                            continue;
                        }

                        if (rawValue != null)
                        {
                            var stream = new MemoryStream(rawValue);
                            value = serializer.Deserialize(stream);
                            return true;
                        }
                    }
                }

                if (lastException != null)
                {
                    throw lastException;
                }
            }

            value = default(T);
            return false;
        }

        public T GetValue(string key)
        {
            T value;
            try
            {
                TryGetValue(key, out value);
                return value;
            }
            catch
            {
                return default(T);
            }
        }

        public void Insert(string key, T value, TimeSpan timeToLive)
        {
            var res = topologyRing.Resolve(key.GetHashCode());
            if (res == null)
            {
                throw new Exception("Topology resolve exception");
            }

            else
            {
                Exception lastException = null;

                var stream = new MemoryStream();
                serializer.Serialize(stream, value);
                
                foreach (var client in res)
                {
                    RedisNativeClient nativeClient;
                    clients.TryGetValue(client, out nativeClient);
                    if (nativeClient != null)
                    {
                        try
                        {
                            nativeClient.SetEx(key, (int)timeToLive.TotalSeconds, stream.ToArray());
                        }
                        catch (Exception exception)
                        {
                            lastException = exception;
                            continue;
                        }
                    }
                }

                if (lastException != null)
                {
                    throw lastException;
                }
            }
        }

        public bool Remove(string key)
        {
            var res = topologyRing.Resolve(key.GetHashCode());
            if (res == null)
            {
                throw new Exception("Topology resolve exception");
            }

            else
            {
                Exception lastException = null;
                bool temp = false;

                foreach (var client in res)
                {
                    RedisNativeClient nativeClient;
                    clients.TryGetValue(client, out nativeClient);
                    if (nativeClient != null)
                    {
                        try
                        {
                            if (nativeClient.Del(key) > 0)
                            {
                                temp = true;
                            }
                        }
                        catch (Exception exception)
                        {
                            lastException = exception;
                            continue;
                        }
                    }
                }

                if (lastException != null)
                {
                    throw lastException;
                }

                return temp;
            }
        }

        public bool Contains(string key)
        {
            var res = topologyRing.Resolve(key.GetHashCode());
            if (res == null)
            {
                throw new Exception("Topology resolve exception");
            }

            else
            {
                Exception lastException = null;

                foreach (var client in res)
                {
                    RedisNativeClient nativeClient;
                    clients.TryGetValue(client, out nativeClient);
                    if (nativeClient != null)
                    {
                        try
                        {
                            if (nativeClient.Exists(key) == 1)
                            {
                                return true;
                            }
                        }
                        catch (Exception exception)
                        {
                            lastException = exception;
                            continue;
                        }
                    }
                }

                if (lastException != null)
                {
                    throw lastException;
                }

                return false;
            }
        }
    }
}
