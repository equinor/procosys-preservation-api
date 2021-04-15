using Equinor.ProCoSys.Preservation.Query.GetUniqueTagRequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetUniqueTagRequirementTypes
{
    [TestClass]
    public class GetUniqueTagRequirementTypesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagRequirementTypesQuery("PX");

            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
