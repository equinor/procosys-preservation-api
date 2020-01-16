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
            var dut = new TimeService();
            
            var time = dut.GetCurrentTimeUtc();
            
            Assert.AreEqual(DateTimeKind.Utc, time.Kind);
        }
    }
}
