using System;
using Equinor.Procosys.Preservation.Query.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.ProjectAggregate
{
    [TestClass]
    public class RequirementDtoTests
    {
        [TestMethod]
        public void Constructor_WithNextDueDate_ShouldSetAllProperties()
        {
            var currentTimeUtc = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var nextDueTimeUtc = currentTimeUtc.AddDays(8);
            var dut = new RequirementDto(1, 2, nextDueTimeUtc, new TimeSpan(8, 0, 0, 0));

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
            var dut = new RequirementDto(0, 0, null, default);

            Assert.IsFalse(dut.NextDueTimeUtc.HasValue);
            Assert.IsNull(dut.NextDueAsYearAndWeek);
            Assert.AreEqual(0, dut.NextDueWeeks);
        }
    }
}
