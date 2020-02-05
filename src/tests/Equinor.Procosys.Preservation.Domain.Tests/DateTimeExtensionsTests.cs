using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        private readonly DateTime _mondayInWeek6 = new DateTime(2020, 2, 3);    // Feb.3
        private readonly DateTime _fridayInWeek6 = new DateTime(2020, 2, 7);    // Feb.7
        private readonly DateTime _sundayInWeek6 = new DateTime(2020, 2, 9);    // Feb.9
        private readonly DateTime _mondayInWeek9 = new DateTime(2020, 2, 24);   // Feb.24
        private readonly DateTime _fridayInWeek9 = new DateTime(2020, 2, 28);   // Feb.28
        private readonly DateTime _sundayInWeek9 = new DateTime(2020, 3, 1);    // Mar.1

        [TestMethod]
        public void GetWeeksReferencedFromStartOfWeek_ShouldBeSameForAllDaysInSameWeek_FromFriday()
        {
            var expected = 3;

            var weeks = _fridayInWeek6.GetWeeksUntil(_mondayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _fridayInWeek6.GetWeeksUntil(_fridayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _fridayInWeek6.GetWeeksUntil(_sundayInWeek9);
            Assert.AreEqual(expected, weeks);
        }

        [TestMethod]
        public void GetWeeksReferencedFromStartOfWeek_ShouldBeSameForAllDaysInSameWeek_FromMonday()
        {
            var expected = 3;

            var weeks = _mondayInWeek6.GetWeeksUntil(_mondayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _mondayInWeek6.GetWeeksUntil(_fridayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _mondayInWeek6.GetWeeksUntil(_sundayInWeek9);
            Assert.AreEqual(expected, weeks);
        }

        [TestMethod]
        public void GetWeeksReferencedFromStartOfWeek_ShouldBeSameForAllDaysInSameWeek_FromSunday()
        {
            var expected = 3;

            var weeks = _sundayInWeek6.GetWeeksUntil(_mondayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _sundayInWeek6.GetWeeksUntil(_fridayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _sundayInWeek6.GetWeeksUntil(_sundayInWeek9);
            Assert.AreEqual(expected, weeks);
        }

        [TestMethod]
        public void GetWeeksReferencedFromStartOfWeek_ShouldBeNegative_WhenFromIsAfterTo()
        {
            var expected = -3;

            var weeks = _sundayInWeek9.GetWeeksUntil(_mondayInWeek6);
            Assert.AreEqual(expected, weeks);

            weeks = _sundayInWeek9.GetWeeksUntil(_fridayInWeek6);
            Assert.AreEqual(expected, weeks);

            weeks = _sundayInWeek9.GetWeeksUntil(_sundayInWeek6);
            Assert.AreEqual(expected, weeks);
        }

        [TestMethod]
        public void GetWeeksReferencedFromStartOfWeek_ShouldBeSame_WithDaysInSameWeek()
        {
            var expected = 0;

            var weeks = _mondayInWeek9.GetWeeksUntil(_mondayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _mondayInWeek9.GetWeeksUntil(_fridayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _mondayInWeek9.GetWeeksUntil(_sundayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _sundayInWeek9.GetWeeksUntil(_mondayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _sundayInWeek9.GetWeeksUntil(_fridayInWeek9);
            Assert.AreEqual(expected, weeks);

            weeks = _sundayInWeek9.GetWeeksUntil(_sundayInWeek9);
            Assert.AreEqual(expected, weeks);
        }
        
        [TestMethod]
        public void GetWeeksReferencedFromStartOfWeek_ShouldBeAYear()
        {
            var nextYear = _mondayInWeek9.AddDays(365);
            var weeks = _mondayInWeek9.GetWeeksUntil(nextYear);
            Assert.AreEqual(52, weeks);
        }

        [TestMethod]
        public void Add2Weeks_ShouldAdd14Days()
        {
            var dt = new DateTime(2020, 1, 7, 2, 3, 4, DateTimeKind.Utc);
            
            dt = dt.AddWeeks(2);

            Assert.AreEqual(2020, dt.Year);
            Assert.AreEqual(1, dt.Month);
            Assert.AreEqual(21, dt.Day);
            Assert.AreEqual(2, dt.Hour);
            Assert.AreEqual(3, dt.Minute);
            Assert.AreEqual(4, dt.Second);
            Assert.AreEqual(DateTimeKind.Utc, dt.Kind);
        }

        [TestMethod]
        public void FormatAsYearAndWeekString_LastDayOf2019_ShouldGiveFirstWeekOfYearAfter()
        {
            var dut = new DateTime(2019, 12, 31);

            Assert.AreEqual("2020w01", dut.FormatAsYearAndWeekString());
        }

        [TestMethod]
        public void FormatAsYearAndWeekString_FirstDayOf2020_ShouldGiveFirstWeekOfYear()
        {
            var dut = new DateTime(2020, 1, 1);

            Assert.AreEqual("2020w01", dut.FormatAsYearAndWeekString());
        }

        [TestMethod]
        public void FormatAsYearAndWeekString_LastDayOf2020_ShouldGiveWeek53()
        {
            var dut = new DateTime(2020, 12, 31);

            Assert.AreEqual("2020w53", dut.FormatAsYearAndWeekString());
        }
    }
}
