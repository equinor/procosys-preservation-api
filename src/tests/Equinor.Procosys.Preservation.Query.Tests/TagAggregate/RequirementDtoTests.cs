using System;
using Equinor.Procosys.Preservation.Query.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.TagAggregate
{
    [TestClass]
    public class RequirementDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var r = new RequirementDto(true, new DateTime(2020, 1, 1));

            Assert.IsTrue(r.NeedsUserInput);
            Assert.AreEqual("2020W01", r.NextDueWeek);
        }
    }
}
