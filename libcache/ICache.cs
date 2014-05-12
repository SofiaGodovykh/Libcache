namespace Kontur.Cache
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface ICache<T>
    {
        T GetValue(string key);

        bool TryGetValue(string key, out T value);

        void Insert(string key, T value, TimeSpan timeToLive);

        bool Remove(string key);

        bool Contains(string key);
    }
}