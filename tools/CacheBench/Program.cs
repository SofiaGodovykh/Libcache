namespace Kontur.Cache.Bench
{
    using Kontur.Cache.Bench.Builders;
    using NDesk.Options;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    public class CacheBenchOptions
    {
        public CacheBenchOptions()
        {
            CacheSize = 100;
            Seed = 123;
            MaxKeyLength = 20;
            CleanInterval = TimeSpan.FromSeconds(5);
            ThreadCount = 8;
            Proportion = 0.5;
            CacheType = "concurrent";
        }

        public int MaxKeyLength { get; set; }

        public string CacheType { get; set; }

        public int ThreadCount { get; set; }

        public int CacheSize { get; set; }

        public double Proportion { get; set; }

        public int Seed { get; set; }

        public TimeSpan CleanInterval { get; set; }
    }

    class Program
    {
        private static CacheBench bench;
        private static Timer timer;
        private static Stopwatch stopwatch = new Stopwatch();
        private static long lastTime = 0;
        private static long lastStatistics;

        public static int Main(string[] args)
        {
            var showHelp = false;
            var options = new CacheBenchOptions();

            var optionSet = new OptionSet();
            optionSet.Add("type=", "Cache type", x => options.CacheType = x);
            optionSet.Add("threadCount=", "Thread count", x => options.ThreadCount = int.Parse(x));
            optionSet.Add("h|help", "show help", x => showHelp = true);
            optionSet.Add("cacheSize=", "Cache size", x => options.CacheSize = int.Parse(x));
            optionSet.Add("proportion=", "Proportion", x => options.Proportion = double.Parse(x));
            optionSet.Add("seed=", "Random seed", x => options.Seed = int.Parse(x));
            optionSet.Add("cleanInterval=", "(sec)", x => options.CleanInterval = TimeSpan.FromSeconds(int.Parse(x)));

            try
            {
                optionSet.Parse(args);

                if (!showHelp)
                {
                    if (string.IsNullOrEmpty(options.CacheType))
                    {
                        throw new OptionException("Missing required value for option \"type\"", "type");
                    }
                }
            }
            catch (OptionException e)
            {
                Console.Write("CacheBench: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try 'CacheBench --help' for more information.");
                return -1;
            }

            if (showHelp)
            {
                ShowHelp(optionSet);
                return 0;
            }

            Execute(options);

            return 0;
        }

        internal static CacheSample<string> CreateSample(CacheBenchOptions options)
        {
            ICacheSampleBuilder<string> sampleBuilder = null;

            switch (options.CacheType)
            {
                case "concurrent":
                    sampleBuilder = new ConcurrentCacheSampleBuilder<string>(options.CacheSize, options.Seed, options.MaxKeyLength, options.CleanInterval);
                    break;

                case null:
                case "simple":
                    sampleBuilder = new CacheSampleBuilder<string>(options.CacheSize, options.Seed, options.MaxKeyLength, options.CleanInterval);
                    break;

                default:
                    throw new ApplicationException(string.Format("Unknown cache type \"{0}\"", options.CacheType));
            }

            return sampleBuilder.Build();
        }

        internal static void Execute(CacheBenchOptions options)
        {
            var cacheSample = CreateSample(options);

            bench = new CacheBench(options.ThreadCount, options.Seed, options.Proportion);
            stopwatch.Start();

            timer = new Timer(OnTimer);
            timer.Change(1000, 1000);

            bench.Run(cacheSample);

            Console.WriteLine("Press any key to exit");
            Console.Read();

            stopwatch.Stop();
            bench.Stop();
            timer.Dispose();

            // вывести затраченное время, кол-во операций, средняя скорость
            Console.WriteLine("Time " + stopwatch.ElapsedMilliseconds);
            var allOperation = bench.GetStatistics().Sum();
            Console.WriteLine("Operations " + allOperation);
            Console.WriteLine("Average speed " + (allOperation / stopwatch.ElapsedMilliseconds));

            Console.ReadLine();
            Console.ReadLine();
        }

        internal static void OnTimer(object state)
        {
            var operations = bench.GetStatistics().Sum();
            var elapsedTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("{0} operations per second", (operations - lastStatistics) / (elapsedTime - lastTime));

            lastTime = elapsedTime;
            lastStatistics = operations;
        }

        private static void ShowHelp(OptionSet optionSet)
        {
            Console.WriteLine("Usage: CacheBench [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
        }
    }
}
