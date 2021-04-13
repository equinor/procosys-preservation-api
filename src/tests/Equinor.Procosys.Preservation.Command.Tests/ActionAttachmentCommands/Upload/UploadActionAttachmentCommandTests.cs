using System;
using System.IO;
using Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ActionAttachmentCommands.Upload
{
    [TestClass]
    public class UploadActionAttachmentCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var content = new MemoryStream();
            var dut = new UploadActionAttachmentCommand(2, 3, "FileName", true, content);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual(3, dut.ActionId);
            Assert.AreEqual(content, dut.Content);
            Assert.AreEqual("FileName", dut.FileName);
            Assert.IsTrue(dut.OverwriteIfExists);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenContentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new UploadActionAttachmentCommand(2, 3, "FileName", true, null));
    }
}
