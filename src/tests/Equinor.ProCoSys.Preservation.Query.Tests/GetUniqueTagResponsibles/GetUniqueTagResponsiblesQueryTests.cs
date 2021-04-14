using Equinor.ProCoSys.Preservation.Query.GetUniqueTagResponsibles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetUniqueTagResponsibles
{
    [TestClass]
    public class GetUniqueTagResponsiblesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagResponsiblesQuery("PX");
            
            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
