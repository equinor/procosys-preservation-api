using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class TagAttachmentTests
    {
        private readonly string TestPlant = "PCS$PlantA";
        private readonly Guid BlobStorageId = new Guid("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetTagAttachmentSpecificProperties()
        {
            var dut = new TagAttachment(TestPlant, BlobStorageId, "FileA");
            Assert.AreEqual($"PlantA/Tag/{BlobStorageId.ToString()}", dut.BlobPath);
            // Other properties are tested in base class
        }
    }
}
