using Equinor.Procosys.Preservation.Query.GetUniqueTagAreas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagAreas
{
    [TestClass]
    public class GetUniqueTagAreasQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagAreasQuery("", "PX");

            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
