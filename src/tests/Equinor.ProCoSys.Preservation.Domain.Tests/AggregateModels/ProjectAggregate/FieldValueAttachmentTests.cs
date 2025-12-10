using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class FieldValueAttachmentTests
    {
        private readonly string _testPlant = "PCS$PlantA";
        private readonly Guid _blobStorageId = new Guid("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetFieldAttachmentSpecificProperties()
        {
            var dut = new FieldValueAttachment(_testPlant, _blobStorageId, "FileA");
            Assert.AreEqual($"PlantA/FieldValue/{_blobStorageId.ToString()}", dut.BlobPath);
            // Other properties are tested in base class
        }

        [TestMethod]
        public void SetFileName_ShouldSetFileName()
        {
            // Assert
            var dut = new FieldValueAttachment(_testPlant, _blobStorageId, "FileA");

            // Act
            dut.SetFileName("FileB");

            // Assert
            Assert.AreEqual("FileB", dut.FileName);
        }
    }
}
