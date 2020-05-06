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
        public void Test_Should()
        {
            // Arrange
            var attachmentOptions = new Mock<IOptionsMonitor<AttachmentOptions>>();
            var options = new AttachmentOptions {MaxSizeKb = 1, BlobContainer = "P"};
            attachmentOptions
                .Setup(x => x.CurrentValue)
                .Returns(options);
            var fileName = "A.txt";
            var blobStorageId = new Guid("{73D5C19C-A7A5-42BC-93AF-979EF5E1C37F}");
            var attachment = new TestAttachment("P", fileName, blobStorageId, null);
            var dut = new BlobPathProvider(attachmentOptions.Object);

            // Act
            var path = dut.CreatePathForAttachment<Abc>(attachment);

            // Assert
            Assert.AreEqual($"{options.BlobContainer}/{nameof(Abc)}/{blobStorageId.ToString()}/{fileName}", path);
        }

        private class Abc
        { }

        private class TestAttachment : Attachment
        {
            public TestAttachment(string plant, string fileName, Guid blobStorageId, string title)
                : base(plant, fileName, blobStorageId, title)
            { }
        }
    }
}
