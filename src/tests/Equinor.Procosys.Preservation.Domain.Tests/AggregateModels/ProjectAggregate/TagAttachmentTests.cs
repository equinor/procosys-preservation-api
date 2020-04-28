using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class TagAttachmentTests
    {
        private TagAttachment _dut;
        private readonly string TestPlant = "PlantA";
        private readonly string Title = "TitleA";
        private readonly string FileName = "FileA";
        private readonly Guid BlobStorageId = new Guid("{6DB281E2-B667-4DCE-B74C-B9A9FBC94964}");

        [TestInitialize]
        public void Setup()
            => _dut = new TagAttachment(TestPlant, BlobStorageId, Title, FileName);

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual(BlobStorageId, _dut.BlobStorageId);
            Assert.AreEqual(FileName, _dut.FileName);
            Assert.AreEqual(Title, _dut.Title);
        }
    }
}
