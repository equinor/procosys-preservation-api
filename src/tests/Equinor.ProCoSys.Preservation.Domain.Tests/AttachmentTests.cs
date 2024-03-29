﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests
{
    [TestClass]
    public class AttachmentTests
    {
        private readonly string TestPlant = "PCS$PlantA";
        private readonly string FileName = "FileA";
        private static string Parent = "Test";
        private readonly Guid BlobStorageId = new("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TestAttachment(TestPlant, BlobStorageId, FileName, Parent);
            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.AreEqual($"PlantA/{Parent}/{BlobStorageId}", dut.BlobPath);
            Assert.AreEqual(FileName, dut.FileName);
        }

        [TestMethod]
        public void GetFullBlobPath_ShouldReturnFullBlobPath()
        {
            // Arrange
            var dut = new TestAttachment(TestPlant, BlobStorageId, FileName, Parent);

            // Act
            var result = dut.GetFullBlobPath();

            // Arrange
            Assert.AreEqual($"PlantA/{Parent}/{BlobStorageId}/{FileName}", result);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenFileNameNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new TestAttachment(TestPlant, BlobStorageId, null, Parent));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenParentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new TestAttachment(TestPlant, BlobStorageId, FileName, null));

        class TestAttachment : Attachment
        {
            public TestAttachment(string plant, Guid blobStorageId, string fileName, string parent)
                : base(plant, blobStorageId, fileName, parent)
            { }
        }
    }
}
