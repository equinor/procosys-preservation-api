using Equinor.Procosys.Preservation.Query.GetUniqueTagJourneys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagJourneys
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
