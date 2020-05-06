using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class AttachmentTests
    {
        private readonly string TestPlant = "PlantA";
        private readonly string Title = "TitleA";
        private readonly string FileName = "FileA";
        private readonly Guid BlobStorageId = new Guid("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TestAttachment(TestPlant, FileName, BlobStorageId, Title);
            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.AreEqual(BlobStorageId, dut.BlobStorageId);
            Assert.AreEqual(FileName, dut.FileName);
            Assert.AreEqual(Title, dut.Title);
        }

        [TestMethod]
        public void Constructor_ShouldSetTitleFromFileName_WhenTitleNotGiven()
        {
            var dut = new TestAttachment(TestPlant, FileName, BlobStorageId, null);
            Assert.AreEqual(FileName, dut.Title);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenFileNameNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new TestAttachment(TestPlant, null, BlobStorageId, "A"));

        class TestAttachment : Attachment
        {
            public TestAttachment(string plant, string fileName, Guid blobStorageId, string title)
                : base(plant, fileName, blobStorageId, title)
            { }
        }
    }
}
