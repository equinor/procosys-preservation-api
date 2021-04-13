using Equinor.ProCoSys.Preservation.Query.GetActions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetActions
{
    [TestClass]
    public class GetActionsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetActionsQuery(1337);

            Assert.AreEqual(1337, dut.TagId);
        }
    }
}
