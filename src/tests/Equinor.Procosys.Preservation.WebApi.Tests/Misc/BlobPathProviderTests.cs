using System;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class BlobPathProviderTests
    {
        [TestMethod]
        public void CreatePathForAttachment_ShouldCreatePath()
        {
            // Arrange
            var attachmentOptions = new Mock<IOptionsMonitor<AttachmentOptions>>();
            var options = new AttachmentOptions {MaxSizeKb = 1, BlobContainer = "P"};
            attachmentOptions
                .Setup(x => x.CurrentValue)
                .Returns(options);
            var fileName = "A.txt";
            var blobStorageId = new Guid("{73D5C19C-A7A5-42BC-93AF-979EF5E1C37F}");
            var attachment = new TestAttachment("P", fileName, blobStorageId);
            var dut = new BlobPathProvider(attachmentOptions.Object);

            // Act
            var path = dut.CreatePathForAttachment(attachment);

            // Assert
            Assert.AreEqual($"{options.BlobContainer}/{fileName}", path);
        }

        private class TestAttachment : Attachment
        {
            public TestAttachment(string plant, string fileName, Guid blobStorageId)
                : base(plant, fileName, blobStorageId)
            { }

            public override string BlobPath => FileName;
        }
    }
}
