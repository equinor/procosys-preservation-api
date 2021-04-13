using Equinor.ProCoSys.Preservation.Query.GetUniqueTagDisciplines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetUniqueTagDisciplines
{
    [TestClass]
    public class GetUniqueTagDisciplinesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagDisciplinesQuery("PX");

            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
