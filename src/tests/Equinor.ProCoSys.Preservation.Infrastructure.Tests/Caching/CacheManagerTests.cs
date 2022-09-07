using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.Infrastructure.Caching;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Tests.Caching
{
    [TestClass]
    public class CacheManagerTests
    {
        private CacheManager _dut;
        private ManualTimeProvider _timeProvider;

        [TestInitialize]
        public void Setup()
        {
            _timeProvider = new ManualTimeProvider(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            TimeService.SetProvider(_timeProvider);
            _dut = new CacheManager();
        }

        [TestMethod]
        public void GetOrCreate_ShouldReturnCachedValue()
        {
            // Act
            var result = _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);
            
            // Assert
            Assert.AreEqual("B", result);
        }

        [TestMethod]
        public void GetOrCreate_ShouldReuseCachedValue_BeforeExpirationExpired()
        {
            // Arrange
            _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);

            // Act
            var result = _dut.GetOrCreate("A", () => "C", CacheDuration.Minutes, 2);
            
            // Assert
            Assert.AreEqual("B", result);
            result = _dut.Get<string>("A");
            Assert.AreEqual("B", result);
        }

        [TestMethod]
        public void GetOrCreate_ShouldReplaceCachedValue_AfterExpirationExpired()
        {
            // Arrange
            _dut.GetOrCreate("A", () => "C", CacheDuration.Seconds, 1);
            _timeProvider.Elapse(TimeSpan.FromSeconds(2));

            // Act
            var result = _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);
            
            // Assert
            Assert.AreEqual("B", result);
            result = _dut.Get<string>("A");
            Assert.AreEqual("B", result);
        }

        [TestMethod]
        public void Get_ShouldReturnCachedValue_WhenKnownKey()
        {
            // Arrange
            _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);
            
            // Act
            var result = _dut.Get<string>("A");
            
            // Assert
            Assert.AreEqual("B", result);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenUnknownKey()
        {
            // Act
            var result = _dut.Get<string>("A");
            
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveKnownKey()
        {
            // Arrange
            _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);
            var result = _dut.Get<string>("A");
            Assert.AreEqual("B", result);

            // Act
            _dut.Remove("A");

            // Assert
            result = _dut.Get<string>("A");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldDoNothing_WhenRemoveUnknownKey() => _dut.Remove("A");

        [TestMethod]
        public async Task GetOrCreate_ShouldReturnCachedValue_Async()
        {
            // Act
            var result = await _dut.GetOrCreate("A", async () => await Concat("C", "D"), CacheDuration.Minutes, 2);

            // Assert
            Assert.AreEqual("CD", result);
        }

        [TestMethod]
        public async Task Get_ShouldReturnCachedValue_Async()
        {
            // Arrange
            await _dut.GetOrCreate("A", async () => await Concat("C", "D"), CacheDuration.Minutes, 2);

            // Act
            var taskResult = _dut.Get<Task<string>>("A");

            // Assert
            Assert.AreEqual("CD", taskResult.Result);
        }

        [TestMethod]
        public async Task GetOrCreate_ShouldReturnCachedValue_Async_BeforeExpirationExpired()
        {
            // Arrange
            await _dut.GetOrCreate("A", async () => await Concat("C", "D"), CacheDuration.Minutes, 2);

            // Act
            var result = await _dut.GetOrCreate("A", async () => await Concat("E", "F"), CacheDuration.Minutes, 2);

            // Assert
            Assert.AreEqual("CD", result);
            var taskResult = _dut.Get<Task<string>>("A");
            Assert.AreEqual("CD", taskResult.Result);
        }

        [TestMethod]
        public async Task GetOrCreate_ShouldReplaceCachedValue_Async_AfterExpirationExpired()
        {
            // Arrange
            await _dut.GetOrCreate("A", async () => await Concat("C", "D"), CacheDuration.Seconds, 1);
            _timeProvider.Elapse(TimeSpan.FromSeconds(2));

            // Act
            var result = await _dut.GetOrCreate("A", async () => await Concat("E", "F"), CacheDuration.Minutes, 2);

            // Assert
            Assert.AreEqual("EF", result);
            var taskResult = _dut.Get<Task<string>>("A");
            Assert.AreEqual("EF", taskResult.Result);
        }

        private static async Task<string> Concat(string s1, string s2)
        {
            var s = string.Empty;
            await Task.Run(() =>
            {
                s = s1 + s2;
            });
            return s;
        }
    }
}
