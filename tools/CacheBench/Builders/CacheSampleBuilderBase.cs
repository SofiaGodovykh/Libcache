namespace Kontur.Cache.Bench.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class CacheSampleBuilderBase<T> : ICacheSampleBuilder<T>
    {
        private readonly Random random;
        private readonly int cacheSize;
        private readonly int maxKeyLength;
        private readonly TimeSpan cleanInterval;

        public CacheSampleBuilderBase(int cacheSize, int seed, int maxKeyLength, TimeSpan cleanInterval)
        {
            this.random = new Random(seed);
            this.cacheSize = cacheSize;
            this.maxKeyLength = maxKeyLength;
            this.cleanInterval = cleanInterval;
        }

        public TimeSpan CleanInterval
        {
            get { return cleanInterval; }
        }

        public CacheSample<T> Build()
        {
            var cache = CreateCache();
            var keys = new List<string>(cacheSize);

            for (var i = 0; i < cacheSize; i++)
            {
                var key = GenerateString();

                if (keys.Contains(key))
                {
                    key = GenerateString();
                }

                keys.Add(key);
                cache.Insert(key, default(T), TimeSpan.FromSeconds(random.Next(1, 30)));
            }

            return new CacheSample<T>(cache, keys);
        }

        protected abstract ICache<T> CreateCache();

        protected string GenerateString()
        {
            var builder = new StringBuilder();
            var length = random.Next(1, maxKeyLength);

            for (int i = 0; i < length; i++)
            {
                var symbol = (char)random.Next(65, 90);
                builder.Append(symbol);
            }

            return builder.ToString();
        }
    }
}