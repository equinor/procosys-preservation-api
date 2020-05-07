using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class AttachmentTests
    {
        private readonly string TestPlant = "PCS$PlantA";
        private readonly string FileName = "FileA";
        private static string Parent = "Test";
        private readonly Guid BlobStorageId = new Guid("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TestAttachment(TestPlant, FileName, BlobStorageId);
            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.AreEqual($"PlantA/{Parent}/{BlobStorageId.ToString()}/{FileName}", dut.BlobPath);
            Assert.AreEqual(FileName, dut.FileName);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenFileNameNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new TestAttachment(TestPlant, null, BlobStorageId));

        class TestAttachment : Attachment
        {
            public TestAttachment(string plant, string fileName, Guid blobStorageId)
                : base(plant, fileName, blobStorageId, Parent)
            { }
        }
    }
}
