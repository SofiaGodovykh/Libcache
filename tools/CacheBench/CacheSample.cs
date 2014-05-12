namespace Kontur.Cache.Bench
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class CacheSample<T>
    {
        private readonly ICache<T> cache;
        private readonly IEnumerable<string> keys;
        private readonly string notContainedKey;

        public CacheSample(ICache<T> cache, IEnumerable<string> keys)
        {
            this.cache = cache;
            this.keys = keys;
            this.notContainedKey = string.Empty;
        }

        public ICache<T> Cache
        {
            get { return cache; }
        }

        public IEnumerable<string> Keys
        {
            get { return keys; }
        }

        public string NotContainedKey
        {
            get
            {
                return notContainedKey;
            }
        }
    }
}