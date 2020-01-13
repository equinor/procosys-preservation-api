using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class GetRequirementTypeByIdQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetsProperties()
        {
            var q = new GetRequirementTypeByIdQuery(1);
            Assert.AreEqual(1, q.Id);
        }
    }
}
