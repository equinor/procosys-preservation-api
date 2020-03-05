using System;
using Equinor.Procosys.Preservation.Domain.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class TimeServiceTests
    {
        [TestMethod]
        public void ReturnsTimeAsUtc()
        {
            var time = TimeService.UtcNow;

            Assert.AreEqual(DateTimeKind.Utc, time.Kind);
        }

        [TestMethod]
        public void Setup_ThrowsException_WhenProviderIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => TimeService.SetProvider(null));
        }

        [TestMethod]
        public void Setup_ThrowsException_WhenProviderDoesNotReturnTimeInUtc()
        {
            Assert.ThrowsException<ArgumentException>(() =>TimeService.SetProvider(new InvalidTimeProvider()));
        }

        public class InvalidTimeProvider : ITimeProvider
        {
            public DateTime UtcNow => new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local);
        }
    }
}
