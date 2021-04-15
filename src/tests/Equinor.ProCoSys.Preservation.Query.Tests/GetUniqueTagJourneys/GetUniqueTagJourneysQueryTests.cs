using Equinor.ProCoSys.Preservation.Query.GetUniqueTagJourneys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetUniqueTagJourneys
{
    [TestClass]
    public class GetUniqueTagJourneysQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetUniqueTagJourneysQuery("PX");

            Assert.AreEqual("PX", dut.ProjectName);
        }
    }
}
