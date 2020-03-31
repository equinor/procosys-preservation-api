using Equinor.Procosys.Preservation.Query.GetTagActionDetails;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetActionDetails
{
    [TestClass]
    public class GetActionDetailsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetActionDetailsQuery("", 56, 1337);

            Assert.AreEqual(56, dut.TagId);
            Assert.AreEqual(1337, dut.ActionId);
        }
    }
}
