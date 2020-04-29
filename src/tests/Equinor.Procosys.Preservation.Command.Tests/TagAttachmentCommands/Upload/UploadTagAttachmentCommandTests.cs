using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagAttachmentCommands.Upload
{
    [TestClass]
    public class UploadTagAttachmentCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UploadTagAttachmentCommand(2, "Title", "File", true);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual("Title", dut.Title);
            Assert.AreEqual("File", dut.FileName);
            Assert.IsTrue(dut.OverwriteIfExists);
        }
    }
}
