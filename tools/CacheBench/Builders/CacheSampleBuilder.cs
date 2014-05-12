namespace Kontur.Cache.Bench.Builders
{
    using System;

    public class CacheSampleBuilder<T> : CacheSampleBuilderBase<T>
    {
        public CacheSampleBuilder(int cacheSize, int seed, int keyLength, TimeSpan cleanInterval)
            : base(cacheSize, seed, keyLength, cleanInterval)
        {

        }

        protected override ICache<T> CreateCache()
        {
            return new CacheDictionary<T>(CleanInterval);
        }
    }
}
