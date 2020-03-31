using Equinor.Procosys.Preservation.Query.GetUniqueTagModes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagModes
{
    [TestClass]
    public class GetUniqueTagModesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagModesQuery("", "PX");

            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
