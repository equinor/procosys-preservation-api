using System;
using System.IO;
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
            var content = new MemoryStream();
            var dut = new UploadTagAttachmentCommand(2, content, "FileName", "Title", true);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual(content, dut.Content);
            Assert.AreEqual("FileName", dut.FileName);
            Assert.AreEqual("Title", dut.Title);
            Assert.IsTrue(dut.OverwriteIfExists);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenFileNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new UploadTagAttachmentCommand(2, null, "FileName", "Title", true));
    }
}
