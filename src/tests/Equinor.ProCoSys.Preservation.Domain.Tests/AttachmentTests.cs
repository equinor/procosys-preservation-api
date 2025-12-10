using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests
{
    [TestClass]
    public class AttachmentTests
    {
        private readonly string _testPlant = "PCS$PlantA";
        private readonly string _fileName = "FileA";
        private static string s_parent = "Test";
        private readonly Guid _blobStorageId = new("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TestAttachment(_testPlant, _blobStorageId, _fileName, s_parent);
            Assert.AreEqual(_testPlant, dut.Plant);
            Assert.AreEqual($"PlantA/{s_parent}/{_blobStorageId}", dut.BlobPath);
            Assert.AreEqual(_fileName, dut.FileName);
        }

        [TestMethod]
        public void GetFullBlobPath_ShouldReturnFullBlobPath()
        {
            // Arrange
            var dut = new TestAttachment(_testPlant, _blobStorageId, _fileName, s_parent);

            // Act
            var result = dut.GetFullBlobPath();

            // Arrange
            Assert.AreEqual($"PlantA/{s_parent}/{_blobStorageId}/{_fileName}", result);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenFileNameNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new TestAttachment(_testPlant, _blobStorageId, null, s_parent));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenParentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new TestAttachment(_testPlant, _blobStorageId, _fileName, null));

        class TestAttachment : Attachment
        {
            public TestAttachment(string plant, Guid blobStorageId, string fileName, string parent)
                : base(plant, blobStorageId, fileName, parent)
            { }
        }
    }
}
