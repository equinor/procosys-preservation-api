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
            var nextDueTimeUtc = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var r = new RequirementDto(nextDueTimeUtc);

            Assert.IsTrue(r.NextDueTimeUtc.HasValue);
            Assert.AreEqual(nextDueTimeUtc, r.NextDueTimeUtc.Value);
            Assert.IsNotNull(r.NextDueAsYearAndWeek);
        }

        [TestMethod]
        public void Constructor_WithoutNextDueDate_ShouldNotSetDueDateProperties()
        {
            var r = new RequirementDto(null);

            Assert.IsFalse(r.NextDueTimeUtc.HasValue);
            Assert.IsNull(r.NextDueAsYearAndWeek);
        }
    }
}
