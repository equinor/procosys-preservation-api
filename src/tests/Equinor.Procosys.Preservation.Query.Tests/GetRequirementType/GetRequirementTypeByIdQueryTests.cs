using Equinor.Procosys.Preservation.Query.GetRequirementType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetRequirementType
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
