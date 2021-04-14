using Equinor.ProCoSys.Preservation.Query.GetTagAttachment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagAttachment
{
    [TestClass]
    public class GetTagAttachmentQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            // Act 
            var dut = new GetTagAttachmentQuery(1, 2);

            // Assert
            Assert.AreEqual(1, dut.TagId);
            Assert.AreEqual(2, dut.AttachmentId);
        }
    }
}
