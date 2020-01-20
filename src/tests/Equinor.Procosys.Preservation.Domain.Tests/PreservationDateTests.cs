using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class PreservationDateTests
    {
        [TestMethod]
        public void LastDayOf2019_ShouldGiveFirstWeekOfYearAfter()
        {
            var dut = new PreservationDate(new DateTime(2019, 12, 31));

            Assert.AreEqual("2020W01", dut.ToString());
        }

        [TestMethod]
        public void FirstDayOf2020_ShouldGiveFirstWeekOfYear()
        {
            var dut = new PreservationDate(new DateTime(2020, 1, 1));

            Assert.AreEqual("2020W01", dut.ToString());
        }

        [TestMethod]
        public void LastDayOf2020_ShouldGiveWeek53()
        {
            var dut = new PreservationDate(new DateTime(2020, 12, 31));

            Assert.AreEqual("2020W53", dut.ToString());
        }
    }
}
