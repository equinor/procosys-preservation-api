using Equinor.Procosys.Preservation.Query.GetTagAttachments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagAttachments
{
    [TestClass]
    public class GetTagAttachmentsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetTagAttachmentsQuery(1337);

            Assert.AreEqual(1337, dut.TagId);
        }
    }
}
