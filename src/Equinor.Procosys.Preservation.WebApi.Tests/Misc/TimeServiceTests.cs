using System;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class TimeServiceTests
    {
        [TestMethod]
        public void ReturnsTimeAsUtc()
        {
            DateTime time = (new TimeService()).GetCurrentTimeUTC();
            Assert.AreEqual(DateTimeKind.Utc, time.Kind);
        }
    }
}
