namespace Kontur.Cache.Bench
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Kontur.Cache;

    public class CacheBench
    {
        private readonly List<Thread> threads = new List<Thread>(); 
        private readonly double proportion;
        private readonly int threadCount;
        private readonly int seed;
        private readonly Statistics statistics;

        private ICache<string> testCache;
        private List<string> testKeys;
        private string miss;
        private volatile bool running;

        public CacheBench(int threadCount, int seed, double proportion)
        {
            this.threadCount = threadCount;
            this.seed = seed;
            this.proportion = proportion;
            this.statistics = new Statistics();

            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(this.Work);
                threads.Add(thread);
            }
        }

        public void Run(CacheSample<string> testInfo)
        {
            this.testCache = testInfo.Cache;
            this.testKeys = new List<string>(testInfo.Keys);
            this.miss = testInfo.NotContainedKey;

            running = true;

            int i = 0;
            
            foreach (var thread in threads)
            {
                thread.Start(i++);
            }
        }

        public void Stop()
        {
            running = false;
            Console.WriteLine("Miss  {0}", this.statistics.Miss);
            Console.WriteLine("Read  {0}", this.statistics.Read);
            Console.WriteLine("Insert  {0}", this.statistics.Insert);
            Console.WriteLine("Remove  {0}", this.statistics.Remove);
        }

        public long[] GetStatistics()
        {
            var result = new long[4] { this.statistics.Miss, this.statistics.Read, this.statistics.Insert, this.statistics.Remove };
            return result;
        }

        private void Work(object state)
        {
            var random = new Random(seed + (int)state);
            bool isInsert = false;
            string removedKey = string.Empty;

            while (running)
            {
                var key = testKeys[random.Next(testKeys.Count)];
                double currentProportion = random.NextDouble();

                if (currentProportion < this.proportion)
                {
                    if (currentProportion <= 0.1 * this.proportion)
                    {
                        testCache.GetValue(miss);
                        Interlocked.Increment(ref this.statistics.Miss);
                    }
                    else
                    {
                        testCache.GetValue(key);
                        Interlocked.Increment(ref this.statistics.Read);
                    }
                }
                else
                {
                    if (isInsert)
                    {
                        testCache.Insert(removedKey, string.Empty, TimeSpan.FromSeconds(random.Next(1, 30)));
                        Interlocked.Increment(ref this.statistics.Insert);
                    }
                    else
                    {
                        testCache.Remove(key);
                        removedKey = key;
                        Interlocked.Increment(ref this.statistics.Remove);
                    }

                    isInsert = !isInsert;
                }
            }
        }

        private class Statistics
        {
            public long Miss;

            public long Read;

            public long Remove;

            public long Insert;
        }
    }
}
