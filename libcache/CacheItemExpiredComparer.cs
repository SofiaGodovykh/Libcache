namespace Kontur.Cache
{
    using System.Collections.Generic;

    internal class CacheItemExpiredComparer<T> : IComparer<CacheItem<T>>
    {
        public int Compare(CacheItem<T> x, CacheItem<T> y)
        {
            return x.ExpiredTime.CompareTo(y.ExpiredTime);
        }
    }
}