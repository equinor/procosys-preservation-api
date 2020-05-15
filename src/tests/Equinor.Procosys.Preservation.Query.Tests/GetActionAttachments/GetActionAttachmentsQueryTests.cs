using Equinor.Procosys.Preservation.Query.GetActionAttachments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetActionAttachments
{
    [TestClass]
    public class GetActionAttachmentsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetActionAttachmentsQuery(1337, 22);

            Assert.AreEqual(1337, dut.TagId);
            Assert.AreEqual(22, dut.ActionId);
        }
    }
}
