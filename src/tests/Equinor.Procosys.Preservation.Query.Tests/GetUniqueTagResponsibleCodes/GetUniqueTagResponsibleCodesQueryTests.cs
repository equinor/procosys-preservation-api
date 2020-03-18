using Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibleCodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagResponsibleCodes
{
    [TestClass]
    public class GetUniqueTagResponsibleCodesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagResponsibleCodesQuery("PX");
            
            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
