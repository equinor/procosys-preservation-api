using Equinor.ProCoSys.Preservation.Query.GetUniqueTagModes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetUniqueTagModes
{
    [TestClass]
    public class GetUniqueTagModesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagModesQuery("PX");

            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
