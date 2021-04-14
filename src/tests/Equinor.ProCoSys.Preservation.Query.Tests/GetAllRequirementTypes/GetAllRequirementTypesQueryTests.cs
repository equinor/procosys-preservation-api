using Equinor.ProCoSys.Preservation.Query.GetAllRequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetAllRequirementTypes
{
    [TestClass]
    public class GetAllRequirementTypesQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var q = new GetAllRequirementTypesQuery(true);
            Assert.IsTrue(q.IncludeVoided);
        }
    }
}
