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
        private static string Parent = "Test";
        private static string Plant = "PCS$PLANT";

        [TestMethod]
        public void CreatePathForAttachment_ShouldCreatePath()
        {
            // Arrange
            var attachmentOptionsMock = new Mock<IOptionsMonitor<AttachmentOptions>>();
            var options = new AttachmentOptions {BlobContainer = "Preservering"};
            attachmentOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(options);
            var fileName = "A.txt";
            var blobStorageId = new Guid("{73D5C19C-A7A5-42BC-93AF-979EF5E1C37F}");
            var attachment = new TestAttachment(Plant, blobStorageId, fileName);
            var dut = new BlobPathProvider(attachmentOptionsMock.Object);

            // Act
            var path = dut.CreateFullBlobPathForAttachment(attachment);

            // Assert
            Assert.AreEqual($"{options.BlobContainer}/{Plant.Substring(4)}/{Parent}/{blobStorageId.ToString()}/{fileName}", path);
        }

        private class TestAttachment : Attachment
        {
            public TestAttachment(string plant, Guid blobStorageId, string fileName)
                : base(plant, blobStorageId, fileName, Parent)
            { }
        }
    }
}
