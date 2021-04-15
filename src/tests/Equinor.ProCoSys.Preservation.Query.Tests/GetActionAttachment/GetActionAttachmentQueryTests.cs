using Equinor.ProCoSys.Preservation.Query.GetActionAttachment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetActionAttachment
{
    [TestClass]
    public class GetActionAttachmentQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            // Act 
            var dut = new GetActionAttachmentQuery(1, 2, 3);

            // Assert
            Assert.AreEqual(1, dut.TagId);
            Assert.AreEqual(2, dut.ActionId);
            Assert.AreEqual(3, dut.AttachmentId);
        }
    }
}
