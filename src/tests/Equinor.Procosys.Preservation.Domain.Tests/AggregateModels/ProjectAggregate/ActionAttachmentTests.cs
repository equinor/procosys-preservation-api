using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class ActionAttachmentTests
    {
        private readonly string TestPlant = "PCS$PlantA";
        private readonly Guid BlobStorageId = new Guid("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestMethod]
        public void Constructor_ShouldSetActionAttachmentSpecificProperties()
        {
            var dut = new ActionAttachment(TestPlant, BlobStorageId, "FileA");
            Assert.AreEqual($"PlantA/Action/{BlobStorageId.ToString()}", dut.BlobPath);
            // Other properties are tested in base class
        }
    }
}
