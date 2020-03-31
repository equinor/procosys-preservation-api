using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.JourneyAggregate
{
    [TestClass]
    public class GetAllJourneysQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new GetAllJourneysQuery("", true);
            Assert.IsTrue(dut.IncludeVoided);
        }
    }
}
