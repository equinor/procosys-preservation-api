using Equinor.Procosys.Preservation.Query.GetRequirementTypeById;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetRequirementTypeById
{
    [TestClass]
    public class GetRequirementTypeByIdQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var q = new GetRequirementTypeByIdQuery(1);
            Assert.AreEqual(1, q.Id);
        }
    }
}
