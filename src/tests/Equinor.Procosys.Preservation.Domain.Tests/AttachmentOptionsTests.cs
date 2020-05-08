using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class AttachmentOptionsTests
    {
        [TestMethod]
        public void ValidFileSuffixesSetter_ShouldInit_ValidFileSuffixArrayGetter()
        {
            // Arrange
            var dut = new AttachmentOptions {ValidFileSuffixes = ".gif|.jpg"};

            // Assert
            Assert.AreEqual(2, dut.ValidFileSuffixArray.Length);
            Assert.AreEqual(".gif", dut.ValidFileSuffixArray[0]);
            Assert.AreEqual(".jpg", dut.ValidFileSuffixArray[1]);
        }
    }
}
