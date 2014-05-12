namespace Kontur.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMemoryCache<T> : ICache<T>
    {
        int Count { get; }

        void Clean();
    }
}