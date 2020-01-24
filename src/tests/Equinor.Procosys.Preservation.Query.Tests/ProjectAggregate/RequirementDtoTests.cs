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
            var r = new RequirementDto(currentTimeUtc, nextDueTimeUtc);

            Assert.IsTrue(r.NextDueTimeUtc.HasValue);
            Assert.AreEqual(nextDueTimeUtc, r.NextDueTimeUtc.Value);
            Assert.IsNotNull(r.NextDueWeeks);
            Assert.AreEqual(1, r.NextDueWeeks.Value);
            Assert.IsNotNull(r.NextDueAsYearAndWeek);
        }

        [TestMethod]
        public void Constructor_WithoutNextDueDate_ShouldNotSetDueDateProperties()
        {
            var r = new RequirementDto(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), null);

            Assert.IsFalse(r.NextDueTimeUtc.HasValue);
            Assert.IsNull(r.NextDueAsYearAndWeek);
            Assert.IsNull(r.NextDueWeeks);
        }
    }
}
