namespace Kontur.Cache.Bench.Builders
{
    using System;

    public class ConcurrentCacheSampleBuilder<T> : CacheSampleBuilderBase<T>
    {
        public ConcurrentCacheSampleBuilder(int cacheSize, int seed, int maxKeyLength, TimeSpan cleanInterval)
            : base(cacheSize, seed, maxKeyLength, cleanInterval)
        {

        }

        protected override ICache<T> CreateCache()
        {
            return new CacheConcurrentDictionary<T>(CleanInterval);
        }
    }
}