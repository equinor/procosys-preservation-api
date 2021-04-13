using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsQueries.GetTags
{
    [TestClass]
    public class RequirementDtoTests
    {
        [TestMethod]
        public void Constructor_WithNextDueDate_ShouldSetAllProperties()
        {
            var nextDueTimeUtc = new DateTime(2020, 6, 2, 14, 2, 16, DateTimeKind.Utc);
            var dut = new RequirementDto(1, "R", RequirementTypeIcon.Battery, nextDueTimeUtc, 3, true);

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("R", dut.RequirementTypeCode);
            Assert.AreEqual(RequirementTypeIcon.Battery, dut.RequirementTypeIcon);
            Assert.IsTrue(dut.NextDueTimeUtc.HasValue);
            Assert.AreEqual(nextDueTimeUtc, dut.NextDueTimeUtc.Value);
            Assert.IsNotNull(dut.NextDueWeeks);
            Assert.IsTrue(dut.NextDueWeeks.HasValue);
            Assert.AreEqual(3, dut.NextDueWeeks);
            Assert.AreEqual("2020w23", dut.NextDueAsYearAndWeek);
            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void Constructor_WithoutNextDueDate_ShouldNotSetDueDateProperties()
        {
            var dut = new RequirementDto(0, null, RequirementTypeIcon.Other, null, null, false);

            Assert.IsFalse(dut.NextDueWeeks.HasValue);
            Assert.IsFalse(dut.NextDueTimeUtc.HasValue);
            Assert.IsNull(dut.NextDueAsYearAndWeek);
        }
    }
}
