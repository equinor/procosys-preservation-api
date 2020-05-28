using Equinor.Procosys.Preservation.Query.GetJourney;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.JourneyAggregate
{
    [TestClass]
    public class GetJourneyByIdQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new GetJourneyByIdQuery(2);
            Assert.AreEqual(2, dut.Id);
        }
    }
}
