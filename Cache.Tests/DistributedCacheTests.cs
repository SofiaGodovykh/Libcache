namespace Kontur.Cache.Tests
{
    using Kontur.Cache.Distributed;
    using NUnit.Framework;
    using ServiceStack.Redis;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    [TestFixture]
    public class DistributedCacheTests
    {
        [Test]
        public void NoActiveTest()
        {
            var topology = new List<Topology>();
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());
            try
            {
               cache.Contains("1");
                Assert.Fail();
            }
            catch
            {
            }
        }

        [Test]
        public void NoActiveTest1()
        {
            var topology = new List<Topology>();
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());
            try
            {
                cache.GetValue("1");
                Assert.Fail();
            }
            catch
            {
            }
        }

        [Test]
        public void NoActiveTest2()
        {
            var topology = new List<Topology>();
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());
            try
            {
                cache.GetValue("1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, string.Empty);
            }

            try
            {
                cache.Contains("1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, string.Empty);
            }
        }

        [Test]
        public void NoActiveTestInsert()
        {
            var topology = new List<Topology>();
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());

            try
            {
                cache.Insert("1", "1", TimeSpan.FromSeconds(4));
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, string.Empty);
            }
        }


        [Test]
        public void NoActiveTestRemove()
        {
            var topology = new List<Topology>();
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());

            try
            {
                cache.Remove("1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, string.Empty);
            }
        }

        [Test]
        public void NoActiveTestTryGet()
        {
            var topology = new List<Topology>();
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());

            try
            {
                string value;
                cache.TryGetValue("1", out value);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, string.Empty);
            }
        }

        [Test]
        public void SomeActiveTest()
        {
            var topology = new List<Topology>();
            topology.Add(new Topology(Int32.MinValue, Int32.MaxValue, new RedisEndPoint("localhost", 6379, 0)));
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());
            try
            {
                Assert.AreEqual(false, cache.Contains("1"));
             //   Assert.Fail();
            }
            catch(Exception e)
            {
             //   Assert.AreEqual(e.Message, string.Empty);
            }
        }

        [Test]
        public void SomeActiveTest1()
        {
            var topology = new List<Topology>();
            topology.Add(new Topology(Int32.MinValue, Int32.MaxValue, new RedisEndPoint("localhost", 6379, 0)));
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());
            cache.Insert("1", "1", TimeSpan.FromSeconds(5));
            Assert.AreEqual(true, cache.Contains("1"));
            string s = null;
            Assert.AreEqual(true, cache.TryGetValue("1", out s));
            Assert.AreEqual(true, cache.Remove("1"));
            Assert.AreEqual(false, cache.Remove("1"));
        }

        [Test]
        public void SomeActiveTest2()
        {
            var topology = new List<Topology>();
            topology.Add(new Topology(Int32.MinValue, Int32.MaxValue, new RedisEndPoint("localhost", 6379, 0)));
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());
            cache.Insert("1", "1", TimeSpan.FromSeconds(50));
            cache.Insert("2", "2", TimeSpan.FromSeconds(50));
            cache.Insert("3", "3", TimeSpan.FromSeconds(50));
            cache.Insert("4", "4", TimeSpan.FromSeconds(50));
            Assert.AreEqual(true, cache.Contains("1"));
            Assert.AreEqual(true, cache.Remove("1"));
            Assert.AreEqual(false, cache.Contains("1"));

            Assert.AreEqual("2", cache.GetValue("2"));
            Assert.AreEqual(true, cache.Contains("2"));
            Assert.AreEqual(true, cache.Remove("2"));
        }

        [Test]
        public void SomeActiveTest3()
        {
            var topology = new List<Topology>();
            topology.Add(new Topology(Int32.MinValue, Int32.MaxValue, new RedisEndPoint("localhost", 6379, 0)));
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());
            cache.Insert("1", "1", TimeSpan.FromSeconds(50));
            cache.Insert("2", "2", TimeSpan.FromSeconds(50));
            cache.Insert("3", "3", TimeSpan.FromSeconds(50));
            cache.Insert("4", "4", TimeSpan.FromSeconds(50));
            Assert.AreEqual(true, cache.Contains("1"));
            Assert.AreEqual(true, cache.Remove("1"));
            Assert.AreEqual(false, cache.Contains("1"));

            Assert.AreEqual("2", cache.GetValue("2"));
            Assert.AreEqual(true, cache.Contains("2"));
            Assert.AreEqual(true, cache.Remove("2"));
        }


        [Test]
        public void SomeActiveTest4()
        {
            var topology = new List<Topology>();
            topology.Add(new Topology(Int32.MinValue, 0, new RedisEndPoint("localhost", 6379, 0)));
            topology.Add(new Topology(0, 32456, new RedisEndPoint("localhost", 6380, 0)));
            topology.Add(new Topology(32456, Int32.MaxValue, new RedisEndPoint("localhost", 6381, 0)));
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());
            cache.Insert("1", "1", TimeSpan.FromSeconds(50));
            cache.Insert("2", "2", TimeSpan.FromSeconds(50));
            cache.Insert("3", "3", TimeSpan.FromSeconds(50));
            cache.Insert("4", "4", TimeSpan.FromSeconds(50));

            Assert.AreEqual("1", cache.GetValue("1"));
            Assert.AreEqual(null, cache.GetValue("qwerty"));

            Assert.AreEqual(true, cache.Contains("1"));
            Assert.AreEqual(true, cache.Remove("1"));
            Assert.AreEqual(false, cache.Contains("1"));

            Assert.AreEqual("2", cache.GetValue("2"));
            Assert.AreEqual(true, cache.Contains("2"));
            Assert.AreEqual(true, cache.Remove("2"));

            string value;
            Assert.AreEqual(true, cache.TryGetValue("3", out value));
            Assert.AreEqual(value, "3");
            Assert.AreEqual(true, cache.Remove("3"));
            Assert.AreEqual(false, cache.Contains("3"));
            Assert.AreEqual(true, cache.Contains("4"));

            Assert.AreEqual(true, cache.Remove("4"));

            Assert.AreEqual(false, cache.Contains("1"));
            Assert.AreEqual(false, cache.Contains("2"));
            Assert.AreEqual(false, cache.Contains("3"));
            Assert.AreEqual(false, cache.Contains("4"));

            Assert.AreEqual(false, cache.Remove("1"));
            Assert.AreEqual(false, cache.Remove("2"));
            Assert.AreEqual(false, cache.Remove("3"));
            Assert.AreEqual(false, cache.Remove("4"));

            Assert.AreEqual(null, cache.GetValue("1"));
            Assert.AreEqual(null, cache.GetValue("2"));
            Assert.AreEqual(null, cache.GetValue("3"));
            Assert.AreEqual(null, cache.GetValue("4"));
        }


        [Test]
        public void ActiveAndInactiveTest()
        {
            var topology = new List<Topology>();
            topology.Add(new Topology(Int32.MinValue, 0, new RedisEndPoint("localhost", 6379, 0)));
            topology.Add(new Topology(0, 32456, new RedisEndPoint("localhost", 6380, 0)));
            topology.Add(new Topology(32456, Int32.MaxValue, new RedisEndPoint("localhost", 6381, 0)));
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());

            Thread.Sleep(60000);

            cache.Insert("1", "1", TimeSpan.FromSeconds(50));
            cache.Insert("2", "2", TimeSpan.FromSeconds(50));
            cache.Insert("3", "3", TimeSpan.FromSeconds(50));
            cache.Insert("4", "4", TimeSpan.FromSeconds(50));

            Assert.AreEqual("1", cache.GetValue("1"));
            Assert.AreEqual(null, cache.GetValue("qwerty"));

            Assert.AreEqual(true, cache.Contains("1"));
            Assert.AreEqual(true, cache.Remove("1"));
            Assert.AreEqual(false, cache.Contains("1"));

            Assert.AreEqual("2", cache.GetValue("2"));
            Assert.AreEqual(true, cache.Contains("2"));
            Assert.AreEqual(true, cache.Remove("2"));

            string value;
            Assert.AreEqual(true, cache.TryGetValue("3", out value));
            Assert.AreEqual(value, "3");
            Assert.AreEqual(true, cache.Remove("3"));
            Assert.AreEqual(false, cache.Contains("3"));
            Assert.AreEqual(true, cache.Contains("4"));

            Assert.AreEqual(true, cache.Remove("4"));

            Assert.AreEqual(false, cache.Contains("1"));
            Assert.AreEqual(false, cache.Contains("2"));
            Assert.AreEqual(false, cache.Contains("3"));
            Assert.AreEqual(false, cache.Contains("4"));

            Assert.AreEqual(false, cache.Remove("1"));
            Assert.AreEqual(false, cache.Remove("2"));
            Assert.AreEqual(false, cache.Remove("3"));
            Assert.AreEqual(false, cache.Remove("4"));

            Assert.AreEqual(null, cache.GetValue("1"));
            Assert.AreEqual(null, cache.GetValue("2"));
            Assert.AreEqual(null, cache.GetValue("3"));
            Assert.AreEqual(null, cache.GetValue("4"));
        }

        [Test]
        public void InactiveTest()
        {
            var topology = new List<Topology>();
            topology.Add(new Topology(Int32.MinValue, 0, new RedisEndPoint("localhost", 6379, 0)));
            topology.Add(new Topology(0, 32456, new RedisEndPoint("localhost", 6380, 0)));
            topology.Add(new Topology(32456, Int32.MaxValue, new RedisEndPoint("localhost", 6381, 0)));
            var topologyRing = new TopologyRing(topology);
            var cache = new DistributedCache<string>(topologyRing, new DefaultBinarySerializer<string>());

            if (1 == 1) { }

            cache.Insert("1", "1", TimeSpan.FromSeconds(50));
            cache.Insert("2", "2", TimeSpan.FromSeconds(50));
            cache.Insert("3", "3", TimeSpan.FromSeconds(50));
            cache.Insert("4", "4", TimeSpan.FromSeconds(50));

            Assert.AreEqual("1", cache.GetValue("1"));
            Assert.AreEqual(null, cache.GetValue("qwerty"));

            Assert.AreEqual(true, cache.Contains("1"));
            Assert.AreEqual(true, cache.Remove("1"));
            Assert.AreEqual(false, cache.Contains("1"));

            Assert.AreEqual("2", cache.GetValue("2"));
            Assert.AreEqual(true, cache.Contains("2"));
            Assert.AreEqual(true, cache.Remove("2"));

            string value;
            Assert.AreEqual(true, cache.TryGetValue("3", out value));
            Assert.AreEqual(value, "3");
            Assert.AreEqual(true, cache.Remove("3"));
            Assert.AreEqual(false, cache.Contains("3"));
            Assert.AreEqual(true, cache.Contains("4"));

            Assert.AreEqual(true, cache.Remove("4"));

            Assert.AreEqual(false, cache.Contains("1"));
            Assert.AreEqual(false, cache.Contains("2"));
            Assert.AreEqual(false, cache.Contains("3"));
            Assert.AreEqual(false, cache.Contains("4"));

            Assert.AreEqual(false, cache.Remove("1"));
            Assert.AreEqual(false, cache.Remove("2"));
            Assert.AreEqual(false, cache.Remove("3"));
            Assert.AreEqual(false, cache.Remove("4"));

            Assert.AreEqual(null, cache.GetValue("1"));
            Assert.AreEqual(null, cache.GetValue("2"));
            Assert.AreEqual(null, cache.GetValue("3"));
            Assert.AreEqual(null, cache.GetValue("4"));
        }
    }
}
