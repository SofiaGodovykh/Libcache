using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kontur.Cache;
using ServiceStack.Redis;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TwoRedices()
        {
            var redis0 = new RedisCache<string>(RedisNativeClient.DefaultHost, RedisNativeClient.DefaultPort, 0, new DefaultBinarySerializer<string>());
            var redis1 = new RedisCache<string>(RedisNativeClient.DefaultHost, 6380, 0, new DefaultBinarySerializer<string>());
            var redis2 = new RedisCache<string>(RedisNativeClient.DefaultHost, 6381, 0, new DefaultBinarySerializer<string>());

            redis0.Insert("0", "a", TimeSpan.FromSeconds(5));
            Assert.AreEqual(true, redis0.Contains("0"));
            Assert.AreEqual(false, redis1.Contains("0"));
        }
    }
}
