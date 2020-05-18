using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class FieldValueAttachmentTests
    {
        private readonly string TestPlant = "PCS$PlantA";
        private readonly Guid BlobStorageId = new Guid("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetFieldAttachmentSpecificProperties()
        {
            var dut = new FieldValueAttachment(TestPlant, BlobStorageId, "FileA");
            Assert.AreEqual($"PlantA/FieldValue/{BlobStorageId.ToString()}", dut.BlobPath);
            // Other properties are tested in base class
        }

        [TestMethod]
        public void SetFileName_ShouldSetFileName()
        {
            // Assert
            var dut = new FieldValueAttachment(TestPlant, BlobStorageId, "FileA");

            // Act
            dut.SetFileName("FileB");

            // Assert
            Assert.AreEqual("FileB", dut.FileName);
        }
    }
}
