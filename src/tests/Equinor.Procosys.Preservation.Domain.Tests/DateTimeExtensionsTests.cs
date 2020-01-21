using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
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
        public void GetWeeksUntilDate_ADay_ShouldGetZeroWeeks()
        {
            var fromTime = new DateTime(2020, 1, 1);
            var toTime = fromTime.AddDays(1);

            var weeks = fromTime.GetWeeksUntilDate(toTime);

            Assert.AreEqual(0, weeks);
        }

        [TestMethod]
        public void GetWeeksUntilDate_OnSameTime_ShouldGetZeroWeeks()
        {
            var fromTime = new DateTime(2020, 1, 1);

            var weeks = fromTime.GetWeeksUntilDate(fromTime);

            Assert.AreEqual(0, weeks);
        }

        [TestMethod]
        public void GetWeeksUntilDate_AWeek_ShouldGetOneWeek()
        {
            var fromTime = new DateTime(2020, 1, 1);
            var toTime = fromTime.AddDays(7);

            var weeks = fromTime.GetWeeksUntilDate(toTime);

            Assert.AreEqual(1, weeks);
        }

        [TestMethod]
        public void GetWeeksUntilDate_OneMinueBeforeAWeek_ShouldGetZeroWeeks()
        {
            var fromTime = new DateTime(2020, 1, 1);
            var toTime = fromTime.AddDays(7).AddMinutes(-1);

            var weeks = fromTime.GetWeeksUntilDate(toTime);

            Assert.AreEqual(0, weeks);
        }

        [TestMethod]
        public void GetWeeksUntilDate_AYear_ShouldGet52Weeks()
        {
            var fromTime = new DateTime(2019, 1, 1);
            var toTime = fromTime.AddYears(1);

            var weeks = fromTime.GetWeeksUntilDate(toTime);

            Assert.AreEqual(52, weeks);
        }

        [TestMethod]
        public void GetWeeksUntilDate_AYearBackward_ShouldGetMinus52Weeks()
        {
            var fromTime = new DateTime(2020, 1, 1);
            var toTime = fromTime.AddYears(-1);

            var weeks = fromTime.GetWeeksUntilDate(toTime);

            Assert.AreEqual(-52, weeks);
        }
    }
}
