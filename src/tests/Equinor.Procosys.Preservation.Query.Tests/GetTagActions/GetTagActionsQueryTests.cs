using Equinor.Procosys.Preservation.Query.GetTagActions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagActions
{
    [TestClass]
    public class GetTagActionsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetTagActionsQuery(1337);

            Assert.AreEqual(1337, dut.TagId);
        }
    }
}
