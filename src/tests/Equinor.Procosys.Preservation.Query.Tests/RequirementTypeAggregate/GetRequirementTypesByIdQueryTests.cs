using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class GetRequirementTypesByIdQueryTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var q = new GetRequirementTypeByIdQuery(1);
            Assert.AreEqual(1, q.Id);
        }
    }
}
