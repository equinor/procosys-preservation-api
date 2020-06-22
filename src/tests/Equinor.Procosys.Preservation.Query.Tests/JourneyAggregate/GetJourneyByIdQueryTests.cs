using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.JourneyAggregate
{
    [TestClass]
    public class GetJourneyByIdQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new GetJourneyByIdQuery(2, true);
            Assert.AreEqual(2, dut.Id);
            Assert.IsTrue(dut.IncludeVoided);
        }
    }
}
