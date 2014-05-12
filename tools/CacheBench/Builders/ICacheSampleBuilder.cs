namespace Kontur.Cache.Bench
{
    public interface ICacheSampleBuilder<T>
    {
        CacheSample<T> Build();
    }
}