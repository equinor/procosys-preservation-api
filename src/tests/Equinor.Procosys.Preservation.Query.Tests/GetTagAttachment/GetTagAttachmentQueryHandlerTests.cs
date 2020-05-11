using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagAttachment;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagAttachment
{
    [TestClass]
    public class GetTagAttachmentQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private TagAttachment _attachment;
        private TestDataSet _testDataSet;
        private int _attachmentId;
        private Mock<IBlobStorage> _blobStorageMock;
        private Uri _uri;
        private string BlobContainer = "bc";
        private Mock<IOptionsMonitor<AttachmentOptions>> _attachmentOptionsMock;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            _blobStorageMock = new Mock<IBlobStorage>();
            _uri = new Uri("http://whatever/file.txt");
            _attachmentOptionsMock = new Mock<IOptionsMonitor<AttachmentOptions>>();
            var options = new AttachmentOptions
            {
                BlobContainer = BlobContainer
            };

            _attachmentOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(options);

            using var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            _testDataSet = AddTestDataSet(context);

            var tag = _testDataSet.Project1.Tags.First();

            _attachment = new TagAttachment(TestPlant, new Guid("{C3412890-1EF8-4E34-B96C-5488200A5AF5}"), "FileA");
            tag.AddAttachment(_attachment);

            context.SaveChangesAsync().Wait();

            _tagId = tag.Id;
            _attachmentId = _attachment.Id;

            var fullBlobPath = _attachment.GetFullBlobPath(BlobContainer);
            _blobStorageMock
                .Setup(b => b.GetDownloadSasUri(fullBlobPath, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(_uri);
        }

        [TestMethod]
        public async Task Handler_ReturnsAttachmentUri()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetTagAttachmentQuery(_tagId, _attachmentId);
            var dut = new GetTagAttachmentQueryHandler(context, _blobStorageMock.Object, _attachmentOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.Ok, result.ResultType);
                
            Assert.AreEqual(result.Data, _uri);
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetTagAttachmentQuery(_tagId, 0);
            var dut = new GetTagAttachmentQueryHandler(context, _blobStorageMock.Object, _attachmentOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }
    }
}
