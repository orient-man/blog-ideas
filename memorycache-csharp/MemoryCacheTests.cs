using System;
using System.Linq;
using App.Common.Caching;
using FluentAssertions;
using NUnit.Framework;
using CacheImpl = System.Runtime.Caching.MemoryCache;

namespace App.Common.Caching
{
    [TestFixture]
    public class MemoryCacheTests
    {
        private readonly CacheImpl _netCache = new CacheImpl("MemoryCacheExtensionsTests");
        private readonly MemoryCache _cache;

        public MemoryCacheTests()
        {
            _cache = new MemoryCache(_netCache);
        }

        [TearDown]
        public void TearDownEachTest()
        {
            _netCache.ToList().ForEach(a => _cache.Remove(a.Key));
        }

        [Test]
        public void add_once_get_many_times()
        {
            // arrange
            var value = new object();
            Func<object> valueFactory = () => value;

            // act & assert
            _cache.AddOrGetExisting("key", valueFactory).Should().Be(value);
            _cache.AddOrGetExisting("key", valueFactory).Should().Be(value);
            _netCache["key"].Should().BeOfType<Lazy<object>>().Subject.Value.Should().Be(value);
        }

        [Test]
        public void when_value_factory_fails_removes_value_from_cache()
        {
            // arrange
            Func<object> valueFactory = () => { throw new Exception(); };

            // act & assert
            Assert.Throws<Exception>(() => _cache.AddOrGetExisting("key", valueFactory));
            _netCache.Contains("key").Should().BeFalse();
        }
    }
}