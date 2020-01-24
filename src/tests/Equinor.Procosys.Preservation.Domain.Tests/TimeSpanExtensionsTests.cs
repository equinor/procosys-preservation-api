using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class TimeSpanExtensionsTests
    {
        [TestMethod]
        public void Weeks_OneDay_ShouldGetZeroWeeks()
        {
            var span = new TimeSpan(1, 0, 0, 0);

            var weeks = span.Weeks();

            Assert.AreEqual(0, weeks);
        }

        [TestMethod]
        public void Weeks_OnSameTime_ShouldGetZeroWeeks()
        {
            var span = new TimeSpan(0, 0, 0, 0);

            var weeks = span.Weeks();

            Assert.AreEqual(0, weeks);
        }

        [TestMethod]
        public void Weeks_OnDefault_ShouldGetZeroWeeks()
        {
            var span = default(TimeSpan);

            var weeks = span.Weeks();

            Assert.AreEqual(0, weeks);
        }

        [TestMethod]
        public void Weeks_AWeek_ShouldGetOneWeek()
        {
            var span = new TimeSpan(7, 0, 0, 0);

            var weeks = span.Weeks();

            Assert.AreEqual(1, weeks);
        }

        [TestMethod]
        public void Weeks_AWeekBefore_ShouldGetOneWeekMinus()
        {
            var span = new TimeSpan(-7, 0, 0, 0);

            var weeks = span.Weeks();

            Assert.AreEqual(-1, weeks);
        }

        [TestMethod]
        public void Weeks_OneDayBefore_ShouldGetZeroWeeks()
        {
            var span = new TimeSpan(-1, 0, 0, 0);

            var weeks = span.Weeks();

            Assert.AreEqual(0, weeks);
        }

        [TestMethod]
        public void Weeks_AYear_ShouldGet52Weeks()
        {
            var span = new TimeSpan(365, 0, 0, 0);

            var weeks = span.Weeks();

            Assert.AreEqual(52, weeks);
        }

        [TestMethod]
        public void Weeks_AYearBackward_ShouldGet52WeeksMinus()
        {
            var span = new TimeSpan(-365, 0, 0, 0);

            var weeks = span.Weeks();

            Assert.AreEqual(-52, weeks);
        }
    }
}
