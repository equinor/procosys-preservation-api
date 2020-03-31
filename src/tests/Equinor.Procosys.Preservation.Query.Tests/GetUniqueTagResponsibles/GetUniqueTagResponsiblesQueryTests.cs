using Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagResponsibles
{
    [TestClass]
    public class GetUniqueTagResponsiblesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagResponsiblesQuery("", "PX");
            
            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
