using Equinor.Procosys.Preservation.Query.GetAllRequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetAllRequirementTypes
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
