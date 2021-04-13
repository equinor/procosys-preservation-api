using Equinor.ProCoSys.Preservation.Query.GetRequirementTypeById;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetRequirementTypeById
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
