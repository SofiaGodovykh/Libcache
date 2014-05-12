namespace Kontur.Cache.Tests
{
    using NUnit.Framework;
    using ServiceStack.Redis;
    using System;
    using System.Threading;

    [TestFixture]
    public class RedisTests
    {
        [Test]
        public void InitTest()
        {
            var redis = new RedisCache<string>(new DefaultBinarySerializer<string>());
            redis.Insert("1", "1", TimeSpan.FromSeconds(1));
        }

        [Test]
        public void AddTest()
        {
            var redis = new RedisCache<string>(new DefaultBinarySerializer<string>());
            redis.Insert("1", "1", TimeSpan.FromSeconds(1));
            Thread.Sleep(3000);
            Assert.AreEqual(false, redis.Contains("1"));
        }

        [Test]
        public void RemoveTest()
        {
            var redis = new RedisCache<string>(new DefaultBinarySerializer<string>());
            redis.Insert("1", "1", TimeSpan.FromSeconds(1));
            Assert.AreEqual(true, redis.Remove("1"));
            Assert.AreEqual(false, redis.Remove("1"));
            redis.Insert("1", "1", TimeSpan.FromSeconds(1));
            Thread.Sleep(2500);
            Assert.AreEqual(false, redis.Remove("1"));
        }

        [Test]
        public void RemoveDoubleTest()
        {
            var redis = new RedisCache<string>(new DefaultBinarySerializer<string>());
            redis.Insert("1", "1", TimeSpan.FromSeconds(1));
            Assert.AreEqual(true, redis.Remove("1"));
            Assert.AreEqual(false, redis.Remove("1"));
            redis.Insert("1", "1", TimeSpan.FromSeconds(1));
            Thread.Sleep(3000);
            Assert.AreEqual(false, redis.Remove("1"));

            redis.Insert("1", "1", TimeSpan.FromSeconds(1));
            redis.Insert("1", "2", TimeSpan.FromSeconds(1));
            Assert.AreEqual(true, redis.Remove("1"));
            Assert.AreEqual(false, redis.Contains("1"));
            redis.Insert("3", "1", TimeSpan.FromSeconds(5));
        }
    }
        
}
