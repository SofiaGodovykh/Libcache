namespace Kontur.Cache
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class QueueCleaner<TKey> where TKey : IComparable
    {
        private readonly IQueue<TKey> queue;

        private readonly int param;

        private readonly ICache cache;

        private readonly long intervalClean;

        private readonly Timer timer;

        public QueueCleaner(int param, IQueue<TKey> pq, ICache cache, long interval)
        {
            this.param = param;
            this.queue = pq;
            this.cache = cache;
            this.intervalClean = interval;
            timer = new Timer(OnTimer);
            timer.Change(intervalClean, intervalClean);
        }


        private void OnTimer(object state)
        {
            this.Clean();
        }

        public void Clean()
        {
            for (int i = 0; i < param; i++)
            {
                cache.Remove(this.queue.Dequeue().ToString());
            }
        }
    }
}
  