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
            var r = new RequirementDto(1, 2, true, 4);

            Assert.AreEqual(1, r.Id);
            Assert.AreEqual(2, r.RequirementDefinitionId);
            Assert.AreEqual(4, r.IntervalWeeks);
            Assert.IsTrue(r.IsVoided);
        }
    }
}
