using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void Add2Weeks_ShouldAdd14Days()
        {
            var dt = new DateTime(2020, 1, 7, 2, 3, 4, DateTimeKind.Utc);
            
            dt = dt.AddWeeks(2);

            Assert.AreEqual(2020, dt.Year);
            Assert.AreEqual(1, dt.Month);
            Assert.AreEqual(21, dt.Date);
            Assert.AreEqual(2, dt.Hour);
            Assert.AreEqual(3, dt.Minute);
            Assert.AreEqual(4, dt.Second);
            Assert.AreEqual(DateTimeKind.Utc, dt.Kind);
        }
    }
}
