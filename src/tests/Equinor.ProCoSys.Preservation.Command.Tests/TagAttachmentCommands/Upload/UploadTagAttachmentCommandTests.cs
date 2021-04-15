using System;
using System.IO;
using Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagAttachmentCommands.Upload
{
    [TestClass]
    public class UploadTagAttachmentCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var content = new MemoryStream();
            var dut = new UploadTagAttachmentCommand(2, "FileName", true, content);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual(content, dut.Content);
            Assert.AreEqual("FileName", dut.FileName);
            Assert.IsTrue(dut.OverwriteIfExists);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenContentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new UploadTagAttachmentCommand(2, "FileName", true, null));
    }
}
