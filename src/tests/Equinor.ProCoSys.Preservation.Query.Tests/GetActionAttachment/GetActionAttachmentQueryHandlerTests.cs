using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetActionAttachment;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetActionAttachment
{
    [TestClass]
    public class GetActionAttachmentQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _actionId;
        private int _attachmentId;
        private readonly DateTime _utcNow = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private ActionAttachment _attachment;
        private TestDataSet _testDataSet;
        private Mock<IBlobStorage> _blobStorageMock;
        private Uri _uri;
        private Mock<IOptionsMonitor<AttachmentOptions>> _attachmentOptionsMock;
        private string BlobContainer = "bc";

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
            var action = new Action(TestPlant, "Open", "Desc1", _utcNow);
            tag.AddAction(action);

            _attachment = new ActionAttachment(TestPlant, Guid.NewGuid(), "FileA");
            action.AddAttachment(_attachment);

            context.SaveChangesAsync().Wait();

            _tagId = tag.Id;
            _actionId = action.Id;
            _attachmentId = _attachment.Id;

            var fullBlobPath = _attachment.GetFullBlobPath(BlobContainer);
            _blobStorageMock
                .Setup(b => b.GetDownloadSasUri(fullBlobPath, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(_uri);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnAttachmentUri()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetActionAttachmentQuery(_tagId, _actionId, _attachmentId);
            var dut = new GetActionAttachmentQueryHandler(context, _blobStorageMock.Object, _attachmentOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.Ok, result.ResultType);
            Assert.AreEqual(result.Data, _uri);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnNotFound_IfActionIsNotFound()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetActionAttachmentQuery(_tagId, _actionId, 0);
            var dut = new GetActionAttachmentQueryHandler(context, _blobStorageMock.Object, _attachmentOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }
    }
}
