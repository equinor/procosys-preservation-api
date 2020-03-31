using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagDetails
{
    [TestClass]
    public class GetTagDetailsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetTagDetailsQuery("", 1337);

            Assert.AreEqual(1337, dut.Id);
        }
    }
}
