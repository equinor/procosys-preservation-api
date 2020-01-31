using System;
using Equinor.Procosys.Preservation.Query.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.ProjectAggregate
{
    [TestClass]
    public class RequirementDtoTests
    {
        private readonly DateTime _currentTimeUtc = new DateTime(2020, 6, 2, 14, 2, 16, DateTimeKind.Utc);

        [TestMethod]
        public void Constructor_WithNextDueDate_ShouldSetAllProperties()
        {
            var nextDueTimeUtc = _currentTimeUtc.AddDays(7);
            var dut = new RequirementDto(1, 2, _currentTimeUtc, nextDueTimeUtc);

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual(2, dut.RequirementDefinitionId);
            Assert.IsTrue(dut.NextDueTimeUtc.HasValue);
            Assert.AreEqual(nextDueTimeUtc, dut.NextDueTimeUtc.Value);
            Assert.IsNotNull(dut.NextDueWeeks);
            Assert.AreEqual(1, dut.NextDueWeeks);
            Assert.IsNotNull(dut.NextDueAsYearAndWeek);
        }

        [TestMethod]
        public void Constructor_WithoutNextDueDate_ShouldNotSetDueDateProperties()
        {
            var dut = new RequirementDto(0, 0, _currentTimeUtc, null);

            Assert.IsFalse(dut.NextDueTimeUtc.HasValue);
            Assert.IsNull(dut.NextDueAsYearAndWeek);
            Assert.AreEqual(0, dut.NextDueWeeks);
        }
    }
}
