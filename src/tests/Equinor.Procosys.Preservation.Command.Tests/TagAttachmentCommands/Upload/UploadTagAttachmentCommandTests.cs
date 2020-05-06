using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagAttachmentCommands.Upload
{
    [TestClass]
    public class UploadTagAttachmentCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var fileMock = new Mock<IFormFile>();
            var dut = new UploadTagAttachmentCommand(2, fileMock.Object, "Title", true);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual(fileMock.Object, dut.File);
            Assert.AreEqual("Title", dut.Title);
            Assert.IsTrue(dut.OverwriteIfExists);
        }
    }
}
