using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class AttachmentTests
    {
        private readonly string TestPlant = "PlantA";
        private readonly string FileName = "FileA";
        private readonly Guid BlobStorageId = new Guid("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TestAttachment(TestPlant, FileName, BlobStorageId);
            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.AreEqual(BlobStorageId, dut.BlobStorageId);
            Assert.AreEqual(FileName, dut.FileName);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenFileNameNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new TestAttachment(TestPlant, null, BlobStorageId));

        class TestAttachment : Attachment
        {
            public TestAttachment(string plant, string fileName, Guid blobStorageId)
                : base(plant, fileName, blobStorageId)
            { }

            public override string BlobPath => FileName;
        }
    }
}
