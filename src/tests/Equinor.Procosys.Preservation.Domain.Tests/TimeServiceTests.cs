using System;
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
        public void Setup_ThrowsException_WhenFuncIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => TimeService.Setup(null));
        }

        [TestMethod]
        public void Setup_ThrowsException_WhenFuncDoenNotReturnTimeInUtc()
        {
            Assert.ThrowsException<ArgumentException>(() => TimeService.Setup(() => new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local)));
        }
    }
}
